using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.EndPoints
{
    public static class TodoRouteGroupExtensions
    {
        // RouteGroupBuilder 확장 메서드
        public static RouteGroupBuilder MapTodosApi(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAllTodos);
            group.MapGet("/complete", GetCompleteTodos);
            group.MapGet("/{id}", GetTodo);
            group.MapPost("/", CreateTodo);
            group.MapPut("/{id}", UpdateTodo);
            group.MapDelete("/{id}", DeleteTodo);

            return group;
        }

        // ===========================
        // 기존 엔드포인트 메서드들
        // ===========================

        static async Task<IResult> GetAllTodos(TodoDb db)
        {
            return TypedResults.Ok(
                await db.Todos
                    .Select(x => new TodoItemDTO(x))
                    .ToArrayAsync());
        }

        static async Task<IResult> GetCompleteTodos(TodoDb db)
        {
            return TypedResults.Ok(
                await db.Todos
                    .Where(t => t.IsComplete)
                    .Select(x => new TodoItemDTO(x))
                    .ToListAsync());
        }

        static async Task<IResult> GetTodo(int id, TodoDb db)
        {
            return await db.Todos.FindAsync(id) is Todo todo
                ? TypedResults.Ok(new TodoItemDTO(todo))
                : TypedResults.NotFound();
        }

        static async Task<IResult> CreateTodo(TodoItemDTO dto, TodoDb db)
        {
            var todo = new Todo
            {
                Name = dto.Name,
                IsComplete = dto.IsComplete
            };

            db.Todos.Add(todo);
            await db.SaveChangesAsync();

            return TypedResults.Created($"{todo.Id}", dto);
        }

        static async Task<IResult> UpdateTodo(int id, TodoItemDTO dto, TodoDb db)
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo is null) return TypedResults.NotFound();

            todo.Name = dto.Name;
            todo.IsComplete = dto.IsComplete;

            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        static async Task<IResult> DeleteTodo(int id, TodoDb db)
        {
            if (await db.Todos.FindAsync(id) is Todo todo)
            {
                db.Todos.Remove(todo);
                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }

            return TypedResults.NotFound();
        }
    }
}
