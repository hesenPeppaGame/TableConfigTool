# TableConfigTool
 Table配置表工具
自动维护 翻译内容流程

1.输出翻译文件： 
原表ClinetLocalization->去重中文->与最终翻译文件比对(是否存在中文key)->未找到的就输出到->待翻译文件

2.填充翻译后的文件: 
新回来的已翻译文件->合并老的已翻译文件->得到最终翻译文件->维护原表ClinetLocalization

文件夹：ClinetLocalization  存放客户端的配置表文件
文件夹：待翻译文件			输出以语言为单位的文件
文件夹：输入已翻译文件		放置翻译后的以语言为单位的文件，格式必须与带翻译文件一致（表头，前三行定义信息，每行之间用Tab键切换的表）
文件夹：已翻译文件 			自动维护已翻译的文件
## 目录配置
- AppScript
	- ConsoleApp
		- AppLib [必要的库]
		- ConsoleApp1 [输出翻译文件]
		- ConsoleApp2 [填充翻译后的文件]
		- FixConfigTool [配置表修复工具]
## 配置表修复
修复开发人员的骚操作导致行列不一致的情况
## 配置表结构
line 1:描述
Line 2:字段类型
line 3:字段描述
Line 4:字段名

=======================================eg.===================================================

表									
string	string	string	string	string	string	string	string		
Key	简体中文	繁体中文	英文	韩文	日文	越南文	泰文	是否在预制体上使用	手动确认了的
Key	Cn	Tw	En	Ko	Ja	Vi	Th	usedInPrefab	confirm