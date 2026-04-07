using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TodoApi.Data;
using TodoApi.EndPoints;
using TodoApi.Filters;
using TodoApi.fromKeyedServices;

// 빌더 생성: 앱에 필요한 설정, 서비스(DI), 환경 설정을 준비하는 단계
var builder = WebApplication.CreateBuilder(args);

// ========== 서비스 등록 영역 (Dependency Injection) ==========

// DB 컨텍스트 등록: 메모리 데이터베이스를 사용하도록 설정 (테스트용)
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
// DB 관련 예외 발생 시 상세한 페이지를 보여주는 필터 추가
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddOpenApi();

// 같은 인터페이스(ICache)를 여러 개 등록할 어떤 구현체를 주입할지 구분하는 기능
builder.Services.AddKeyedSingleton<ICache, BigCache>("big");
builder.Services.AddKeyedSingleton<ICache, SmallCache>("small");

// 앱 빌드: 위에서 설정한 서비스들을 바탕으로 실제 웹 애플리케이션 객체 생성
var app = builder.Build();



// ========== 미들웨어 및 엔드포인트 설정 영역 ==========

app.MapGet("/", () => "Hello World!");

// Logging
app.Logger.LogInformation("The app started");

// 에러 처리 미들웨어: 개발 환경이 아닐 때 예외가 나면 "/oops" 경로로 사용자에게 안내
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();          // /openapi/v1.json 경로 생성
    app.MapScalarApiReference(); // /scalar/v1 경로로 문서 시각화
}
else
{
    app.UseExceptionHandler("/oops");
}

// 그룹화: "/todoitems"로 시작하는 공통 경로를 가진 엔드포인트들을 묶습니다.
var todoItems = app.MapGroup("/todoitems");

// 설정(Configuration) 접근: appsettings.json 파일 등에 정의된 값을 읽어옵니다.
var message = app.Configuration["HelloKey"] ?? "Configfailed!";
app.MapGet("/config", () => message);

app.MapGet("/oops", () => "Oops! An error happened.");

// Keyed Services: 매개변수 앞에 [FromKeyedServices("키")]를 붙여 원하는 객체를 가져옴
app.MapGet("/big", ([FromKeyedServices("big")] ICache bigCache) => bigCache.Get("date"));
app.MapGet("/small", ([FromKeyedServices("small")] ICache smallCache) => smallCache.Get("date"));

// 외부 파일에서 정의한 엔드포인트 매핑 호출 (코드가 길어질 때 분리하는 기법)
TodosEndpoints.Map(app);

// catch-all 라우트
// 특정 경로 뒤에 오는 나머지 문자열을 한 번에 찾아서 처리하는 라우팅 방식
app.MapGet("/posts/{*rest}", (string rest) => $"Routing to {rest}");



// ========== 고급 그룹화 및 보안 설정 ==========

// 공개용 API 그룹
app.MapGroup("/public/todos")
    .MapTodosApi() // 확장 메서드로 정의된 API들 매핑
    .WithTags("Public"); // Swagger(API 문서)에서 그룹화할 태그명

// 비공개용 API 그룹 (보안 적용)
app.MapGroup("/private/todos")
    .MapTodosApi()
    .WithTags("Private")
    .AddEndpointFilterFactory(TodoFilters.QueryPrivateTodos) // 커스텀 로직 필터(검증 등) 추가
    .RequireAuthorization(); // 로그인(인증)된 사용자만 접근 가능하도록 제한

app.Run();