Vending Machine (MVP)
=====================

Assumptions
===========

1. Vending machine is used by one person at a time, therefore no multi threading issues will be considered here.
2. All coin denomination can be represented by an integer value and the differences from the actual
currency amount is represented by a scale (eg. scale = 2, when 0.20p is equal to 20)
3. Vending machine only has a finite limit the number of different coin denominations and how much 
4. If there is an error happen with the vending machine, the error will be logged and we assumed someone will have to handle
it offline. Further enhancement could be involve sending SMS message to vendor.
5. Due to time constraint product inventory is not model in this exercise. We assumed that the
product is always available.
6. Due to time constraint, I didn't have time to finish all the unit tests. I have spend 
time on the Console App so that non technical user can also evaluate the system and provide feedback in the early stage. (MVP)


How to use (Required .net core sdk v2 (minimum))
================================================
1. dotnet restore VendingMachineSimulator.sln
2. dotnet build VendingMachineSimulator.sln
3. cd VendingMachineSimulator\bin\Debug\netcoreapp2.2
4. dotnet VendingMachineSimulator.dll



