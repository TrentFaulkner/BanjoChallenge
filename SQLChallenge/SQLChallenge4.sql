use Dev;

/*
Q4

Most likely there will be many more reads than writes in this table, so it would be of most benefit to
make reads more efficient.

Unfortunately union all can't be included in views, so the Person table could have the Heirarchy column added
to save calculating the node structure on every select
With this example, one write has the potential to require every row to be updated O(n), however the reads will
be much faster.

Assuming all rows can only have one parent, then the clustered index could be changed to be on Id, ParentId as
most queries would include both columns.  Having this index could also prevent circular references, if a hierarchy
column was calculated, again if this is intended.

It really does depend on what queries are going to be used though.  For instance, if you wanted all the children
of a node, but you didn't need to know the hierarchy or whether the selected node is a top level parent,
then you wouldn't need to calculate the hierarchy at all and it would just be a tree traversal.

It also depends very much on whether the circular reference is a 'mistake' and should be dealt with, or if 
it's a common, and necessary occurrence.

*/

