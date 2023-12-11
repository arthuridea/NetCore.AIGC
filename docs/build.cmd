:: 自动构建文档
:: 使用方法: build {version} [{package name}]
:: {version} 生成的文档版本号，对应的api文档会输出到 v{version} 文件夹
:: {package name} 对应的包名，指定此参数会将文档发布到 %NUGETPACKAGE_REF%{package name}文件夹下
@echo off 
chcp 65001 
::echo 【INFO】清除缓存...
::rd /s /q .\obj\.cache
echo 【INFO】清除过期文件...
echo 【INFO】清除.\dist\api
rd /s /q .\dist\api
echo 【INFO】清除.\dist\_site
rd /s /q .\dist\_site
echo 【INFO】重建元数据...
docfx metadata docfx.json
echo 【INFO】构建文档...
docfx build docfx.json

::if "%2" == "" (
::    echo 【WARNING】未指定发布包名 跳过发布.
::) else (
::    echo 【INFO】发布新文档到%NUGETPACKAGE_REF%%2 ...
::    xcopy /y /s _dist %NUGETPACKAGE_REF%%2
::)