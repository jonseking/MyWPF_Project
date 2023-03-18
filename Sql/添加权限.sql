select* from sys_menu where ParentId=24
select * from sys_auth
SELECT * FROM SYS_ROLE
SELECT * FROM SYS_RoleAuth
select* from SYS_USER 
UPDATE  SYS_USER SET RoleID=1 WHERE UserName='admin';
--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '系统管理',0,99,1;


--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '基础信息管理',0,1,1;

--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '数据查询',0,2,1;

--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '菜单管理',30,1,1;
--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '角色管理',30,2,1;
--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '用户管理',30,3,1;
--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '权限管理',30,4,1;
--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '备份数据',30,5,1;

--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '选择供应商',31,1,1;

--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '采购查询',32,1,1;

--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '按供应商查询',39,1,1;

--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '按仓库查询',39,2,1;

--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '按商品查询',39,3,1;

--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '配送查询',32,2,1;

--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '按客户查询',1002,1,1;

--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '按仓库查询',1002,2,1;

--insert into [dbo].[SYS_AUTH] (authname,parentid,authindex,isusing) 
--select '按商品查询',1002,3,1;




