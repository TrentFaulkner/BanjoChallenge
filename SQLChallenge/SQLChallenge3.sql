use Dev;

-- Q3a
-- Technically, Id:1, ParentId: 8 should not be included in the resultset as it is a child of Id:8
-- But this will give the resultset asked for

declare @id as int
select @id = 8;

with Relations(Id, ParentId, Hierarchy, HasCircularReference) as
(
	select Id, ParentID, cast(concat(':', Id, ':') as varchar(max)), 0
	from Person
	where Id <= 2

	union all

	select NextGen.Id, NextGen.ParentId
	, cast(concat(Parent.Hierarchy, NextGen.Id, ':') as varchar(max))
	, iif (Parent.Hierarchy like concat(':',NextGen.id,'%'), 1, 0) as HasCircularReference
	from Person as NextGen inner join Relations as Parent on NextGen.ParentId = Parent.Id
	where HasCircularReference != 1
)
-- Firstly, get the hierarchy that contains the @id, starting from the parent down, and then select every node in that hierarchy
select * from Relations where id in
(
	select * from string_split (
		(select Hierarchy from Relations where Id  = @id), ':'
	)
)
and id != @id
and HasCircularReference = 0


-- Q3b
/* There are multiple solutions to this problem that give the expected resultset.
 However, it is unknown how numbers other than #2 are to be treated, and how the hierarchy
 is meant to display, if your @id isn't a top level parent.
 I have assumed that the Heirarchy always has a top level parent first and that
 #8 is a child first, and not a parent.
*/
--declare @id as int
select @id = 2;

with Relations(Id, ParentId, Hierarchy, HasCircularReference) as
(
	select Id, ParentID, cast(concat(':', Id, ':') as varchar(max)), 0
	from Person
	where Id <= 2

	union all

	select NextGen.Id, NextGen.ParentId
	, cast(concat(Parent.Hierarchy, NextGen.Id, ':') as varchar(max))
	, iif (Parent.Hierarchy like concat(':',NextGen.id,'%'), 1, 0) as HasCircularReference
	from Person as NextGen inner join Relations as Parent on NextGen.ParentId = Parent.Id
	where HasCircularReference != 1
)
select * from Relations
where Hierarchy like concat('%:',@id,':_%')
and HasCircularReference = 0
