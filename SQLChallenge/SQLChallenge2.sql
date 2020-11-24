use Dev;


-- Q2
-- UPDATE dbo.Person SET ParentId = 8 WHERE Id = 1;
with Relations(Id, ParentId, Hierarchy, HasCircularReference)
as
(
	select Id, ParentId, cast(concat(':', Id, ':') as varchar(max)), 0
	from Person as FirstGen
	where Id <= 2
	
	union all

	select NextGen.Id, Parent.Id
	, cast(concat(Parent.Hierarchy, NextGen.Id, ':') as varchar(max))
	, iif (Parent.Hierarchy like concat(':',NextGen.id,'%'), 1, 0) as HasCircularReference
	from Person as NextGen inner join Relations as Parent on NextGen.parentId = Parent.Id
	where HasCircularReference != 1
)
select * from Relations
order by Hierarchy
option (maxrecursion 100)

