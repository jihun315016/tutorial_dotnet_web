using TodoApi.Data;

namespace TodoApi.Filters
{
    public class TodoFilters
    {
        /// <summary>
        /// /private/todos 그룹에만 적용되는 필터
        /// DbContext를 "Private 모드"로 설정한다.
        /// </summary>
        public static EndpointFilterDelegate QueryPrivateTodos(
            EndpointFilterFactoryContext factoryContext,
            EndpointFilterDelegate next)
        {
            var dbContextIndex = -1;

            // 엔드포인트 메서드의 파라미터 중 TodoDb 위치 탐색
            foreach (var parameter in factoryContext.MethodInfo.GetParameters())
            {
                if (parameter.ParameterType == typeof(TodoDb))
                {
                    dbContextIndex = parameter.Position;
                    break;
                }
            }

            // TodoDb가 없는 엔드포인트는 필터 적용 없이 통과
            if (dbContextIndex < 0)
            {
                return next;
            }

            // 실제 요청 처리 시 실행될 delegate 반환
            return async invocationContext =>
            {
                var dbContext = invocationContext.GetArgument<TodoDb>(dbContextIndex);

                // Private API 진입 시점
                dbContext.IsPrivate = true;

                try
                {
                    return await next(invocationContext);
                }
                finally
                {
                    // 요청 종료 시 원상 복구
                    // (DbContext 풀링/재사용 대비)
                    dbContext.IsPrivate = false;
                }
            };
        }
    }
}
