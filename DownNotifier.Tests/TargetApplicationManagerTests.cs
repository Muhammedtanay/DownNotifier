using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using AutoMapper;
using DownNotifier.Business.Services;
using DownNotifier.DataAccess.Abstract;
using DownNotifier.Entities;
using DownNotifier.ViewModels;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace DownNotifier.Tests
{
    public class TargetApplicationManagerTests
    {
        private readonly Mock<ITargetApplicationRepository> _repoMock;
        private readonly Mock<INotificationService> _notificationMock;
        private readonly Mock<ILogger<TargetApplicationManager>> _loggerMock;
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly HttpClient _httpClient;
        private readonly TargetApplicationManager _service;
        private readonly Mock<IRecurringJobManager> _recurringJobMock;


        public TargetApplicationManagerTests()
        {
            _repoMock = new Mock<ITargetApplicationRepository>();
            _notificationMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<TargetApplicationManager>>();

            var store = new Mock<IUserStore<AppUser>>();
            _userManagerMock = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);

            _mapperMock = new Mock<IMapper>();

            var handlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(handlerMock.Object);

            _recurringJobMock = new Mock<IRecurringJobManager>();

            _service = new TargetApplicationManager(
                _repoMock.Object,
                _notificationMock.Object,
                _loggerMock.Object,
                _userManagerMock.Object,
                _httpClient,
                _mapperMock.Object,
    _recurringJobMock.Object
            );

        }

        [Fact]
        public async Task AddAsync_Should_CallRepositoryAddAndScheduleJob()
        {

            var vm = new TargetApplicationViewModel { CheckIntervalInMinutes = 5 };
            var entity = new TargetApplication { Id = 1 };
            _mapperMock.Setup(m => m.Map<TargetApplication>(vm)).Returns(entity);
            _repoMock.Setup(r => r.AddAsync(entity)).Returns(Task.CompletedTask);

            await _service.AddAsync(vm, "user1");

            _repoMock.Verify(r => r.AddAsync(entity), Times.Once);
           
            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), 
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);

        }

        [Fact]
        public async Task CheckTargetsAsync_Should_Notify_When_ResponseIsNotSuccess()
        {

            var targetId = 1;
            var target = new TargetApplication
            {
                Id = targetId,
                Url = "http://example.com",
                UserId = "user1",
                Name = "TestTarget"
            };

            _repoMock.Setup(r => r.GetByIdAsync(targetId)).ReturnsAsync(target);


            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            var httpClient = new HttpClient(handlerMock.Object);

            var serviceWithMockHttp = new TargetApplicationManager(
                _repoMock.Object,
                _notificationMock.Object,
                _loggerMock.Object,
                _userManagerMock.Object,
                httpClient,
                _mapperMock.Object,
                _recurringJobMock.Object);

            _userManagerMock.Setup(u => u.FindByIdAsync(target.UserId))
                .ReturnsAsync(new AppUser { Email = "test@example.com" });

            _notificationMock.Setup(n => n.NotifyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _repoMock.Setup(r => r.UpdateAsync(target)).Returns(Task.CompletedTask);

            await serviceWithMockHttp.CheckTargetsAsync(targetId);

            _notificationMock.Verify(n => n.NotifyAsync("test@example.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _repoMock.Verify(r => r.UpdateAsync(target), Times.Once);
          
            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true), 
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.Once);

        }
    }
}