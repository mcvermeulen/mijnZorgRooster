﻿using Microsoft.AspNetCore.Mvc;
using mijnZorgRooster.Controllers;
using mijnZorgRooster.DAL;
using mijnZorgRooster.DAL.Repositories;
using mijnZorgRooster.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace mijnZorgRooster.Tests.Controllers
{
    public class DienstControllerTest
    {
        private Mock<IUnitOfWork> _unitOfWork;
        private Mock<IDienstRepository> _dienstRepository;

        public DienstControllerTest()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _dienstRepository = new Mock<IDienstRepository>();
        }

        [Fact]
        public async Task Index_ReturnsAViewResult_WithAListOfDienstDTOs()
        {
            //Arrange
            _dienstRepository.Setup(repo => repo.GetAsync()).Returns(Task.FromResult(GetDiensten()));
            var controller = new DienstController(_unitOfWork.Object, _dienstRepository.Object);

            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<DienstDTO>>(
                viewResult.ViewData.Model);
            Assert.Equal(3, model.Count());
        }

        [Fact]
        public async Task Detail_ReturnsAViewResult_WithADienstDTO()
        {
            //Arrange
            int dienstID = 1;
            DienstDTO dienst = (GetDiensten().FirstOrDefault(c => c.DienstID == dienstID));

            _dienstRepository.Setup(repo => repo.GetByIdAsync(dienstID)).Returns(Task.FromResult(dienst));
            var controller = new DienstController(_unitOfWork.Object, _dienstRepository.Object);

            //Act
            var result = await controller.Details(dienstID);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<DienstDTO>(
                    viewResult.ViewData.Model);

            Assert.Equal(1, model.DienstID);
        }

        [Fact]
        public async Task Details_ReturnsANotFound()
        {
            //Arrange
            var controller = new DienstController(_unitOfWork.Object, _dienstRepository.Object);

            //Act
            var result = await controller.Details(null);

            //Assert
            var contentResult = Assert.IsType<NotFoundResult>(result);
        }

        private List<DienstDTO> GetDiensten()
        {
            List<DienstDTO> diensten = new List<DienstDTO>
            {
                new DienstDTO()
                {
                    DienstID = 1,
                    Datum = DateTime.Parse("01-01-2019"),
                    DienstProfielID = 1,
                    Beschrijving = "Beschrijving Dienstprofiel",
                    Begintijd = TimeSpan.Parse("07:00"),
                    Eindtijd = TimeSpan.Parse("14:00"),
                    MinimaleBezetting = 5
                },
                new DienstDTO()
                {
                    DienstID = 2,
                    Datum = DateTime.Parse("02-01-2019"),
                    DienstProfielID = 1,
                    Beschrijving = "Beschrijving Dienstprofiel",
                    Begintijd = TimeSpan.Parse("07:00"),
                    Eindtijd = TimeSpan.Parse("14:00"),
                    MinimaleBezetting = 5
                },
                new DienstDTO()
                {
                    DienstID = 3,
                    Datum = DateTime.Parse("03-01-2019"),
                    DienstProfielID = 1,
                    Beschrijving = "Beschrijving Dienstprofiel",
                    Begintijd = TimeSpan.Parse("07:00"),
                    Eindtijd = TimeSpan.Parse("14:00"),
                    MinimaleBezetting = 5
                }
            };
            return diensten;
        }
    }

   
}

   



    

