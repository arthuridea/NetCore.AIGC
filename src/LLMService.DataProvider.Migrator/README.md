# 项目说明

本项目为独立的数据库迁移项目,支持在`appsettings.json`中配置数据库提供程序，目前支持`SqlServer`和`MySql`.

## 目录结构

例如，有3次迁移Migration1,Migration2,Migration3,文件结构大致如下：

```
.
├── Migrations /*迁移文件夹*/
│   ├── 时间戳1_Migration1.cs /*第1次迁移*/
│   ├── 时间戳2_Migration2.cs /*第2次迁移*/
│   ├── 时间戳3_Migration3.cs /*第3次迁移*/ 
│   ├── ChatDbContextModelSnapshot.cs /*迁移快照文件*/
│   └── Scripts /*迁移脚本文件夹*/
│       ├── Migration1.sql /*第1次迁移脚本*/
│       ├── Migration2.sql /*第2次迁移脚本*/
│       └── Migration3.sql /*第3次迁移脚本*/
├── ChatDbContextFactory.cs /*迁移配置程序*/
└── README.md /*说明文件*/

```

#### 创建迁移

以`NewMigration`为迁移名称

##### VisualStudio 包管理器命令行
```
add-migration -c ChatDbContext NewMigration
```

#### 创建迁移脚本

`NewMigration`为本次迁移名称，`LastMigration`为上次迁移名称（可选），加入`-from` 参数创建`LastMigration`之后所做的增量脚本

##### VisualStudio 包管理器命令行

需要注意当前命令行执行的目录.

+ 仅创建迁移增量脚本
```
script-migration -o .\src\LLMService.DataProvider.Migrator\Migrations\Scripts\NewMigration.sql -Idempotent  -from LastMigration
```

+ 创建整个数据库的建库脚本

`DbProvider`参数可选值：SqlServer,MySql
```
script-dbcontext -c ChatDbContext -Args "--DbProvider SqlServer"
```
