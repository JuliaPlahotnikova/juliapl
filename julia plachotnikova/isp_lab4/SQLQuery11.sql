CREATE PROCEDURE [dbo].[GetOrders]
(
	@Count             INT = 25
)
	AS
	BEGIN
		SELECT TOP (@Count)
			[Orders].[OrderID],		 
			[Employees].[FirstName],
			[Employees].[LastName],
			[Orders].[ShipCountry],
			[Orders].[ShipCity],
			[Orders].[ShipAddress],
			[Orders].[Freight],
			[Orders].[ShipName],
			[Customers].[ContactName],
			[Customers].[CompanyName],
			[Customers].[Phone]
			--[Territories].RegionID
		FROM [Customers]
			join [Orders] ON [Customers].[CustomerID] = [Orders].[CustomerID]
			join [Employees] ON [Orders].[EmployeeID] = [Employees].[EmployeeID]
			join [Shippers] ON [Orders].[ShipVia] = [Shippers].[ShipperID]
		ORDER BY [OrderID]
	END
