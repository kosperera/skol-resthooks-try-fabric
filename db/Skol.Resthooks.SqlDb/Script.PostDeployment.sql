-- This file contains SQL statements that will be executed after the build script.
-- Embed subscripts using :r .\Path-to-SQL-file
-- Make sure to exclude from post-deployment build step.
-- <Build Remove="Path-to-SQL-file"/>

MERGE INTO dbo.Topics AS [to]
USING (VALUES (11, 'KitchenSync.Intent.Confirmation', 1, 1),
              (12, 'KitchenSync.Intent.Notification', 1, 0),

              (21, 'Menu.Notification', 0, 0),

              (31, 'Inventory.CountUpdated', 0, 0),

              (41, 'Store.Offline', 0, 0),
              (42, 'Store.Online', 0, 0),

              (51, 'Order.Placed', 0, 1),
              (52, 'Order.Notification', 0, 0),
              (53, 'Order.Accepted', 0, 0),
              (54, 'Order.Denied', 0, 0),
              (55, 'Order.Cancelled', 0, 0),
              (56, 'Order.Ready', 0, 0),

              (61, 'Bookings.Notification', 0, 0),
              (62, 'Bookings.Accepted', 0, 0),
              (63, 'Bookings.Cancelled', 0, 0),

              (71, 'Team.Shift.Notification', 0, 0),
              (72, 'Team.Shift.Ended', 0, 0),
              (73, 'Team.Shift.Cancelled', 0, 0)) AS [from] (Id, [Name], IsSystemKind, Archived)
ON [to].Id = [from].Id
WHEN MATCHED THEN
    UPDATE SET [Name] = [from].Name,
               IsSystemKind = [from].IsSystemKind,
               Archived = [from].Archived
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, [Name], IsSystemKind, Archived) VALUES (Id, [Name], IsSystemKind, Archived);
