FROM microsoft/dotnet:2.1-aspnetcore-runtime
COPY . /app
WORKDIR /app
ENV ASPNETCORE_URLS=http://127.0.0.1:8080
ENTRYPOINT ["dotnet", "NodaTime.Web.dll"]
