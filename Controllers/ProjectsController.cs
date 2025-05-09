﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTracker.Models.DTO;
using TaskTracker.Models;
using TaskTracker.Service;
using TaskTracker.db;
using Microsoft.EntityFrameworkCore;

namespace TaskTracker.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectOperateService _service;

        public ProjectsController(IProjectOperateService service)
        {
            _service = service;
        }

        private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject(CreateProjectDTO dto)
        {
            var project = await _service.CreateNewProjectAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetProjectById),           
                                   new { id = project.ProjectId },   
                                   project);
        }

        [Authorize]
        [HttpPost("members")]
        public async Task<IActionResult> AddMember(AddUsersToProjectDTO dto)
        {
            try
            {
                await _service.AddUserToProjectAsync(dto, CurrentUserId);
            }catch (UnauthorizedAccessException e)
            {
                return BadRequest("у пользователя недостаточно прав для добавления людей в проект");
            }
                return NoContent();
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Project>> GetProjectById(int id)
        {
            var project = await _service.GetProjectById(id);

            return project is null ? NotFound() : Ok(project);
        }
    }
}
