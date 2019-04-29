# 声明使用的基础镜像
FROM microsoft/aspnetcore:2.0-nanoserver-1803 AS base 
# 设置工作目录
WORKDIR /app
# 将本地应用拷贝到 容器 /app/ 目录下 "./"代表的就是/app 因为dockerfile文件和程序文件都在发布文件夹下 所以两个目录是一样的
COPY ./ ./
# 设置导出端口
EXPOSE 5000/tcp
# 指定应用入口点 NetCore.dll代表的是主程序文件
ENTRYPOINT ["dotnet", "ZsqApp.Core.WebApi.dll"]