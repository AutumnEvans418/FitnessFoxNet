# Fitness Fox
A local, crossplatform, and secure fitness app built in Blazor .NET 8 that supports iOS, Android, and Web.

> This project is still a work in progress and likely contains many bugs.

## Why
After going through some important life events, I've started to focus more on my health and I wanted to start tracking my health data. After looking at various fitness apps out there, I decided that I would just store my data in google sheets as at the time I didn't need anything complicated and didn't want to pay for a service. I also needed to track less common data than what most health apps typically allowed for. Eventually, I started to build an app around that data and created a way to automatically syncronize the data in the app to my google spreadsheet, hence that's why this project exists.

## Why the Name?
Because foxes are cute and fit.

## Getting Started
- Clone the project
- Run `dotnet build` to build the project
- Run `dotnet test` to run the unit tests

## Features
- [x] Create User Accounts (Mainly used when deploying to the web).
- [x] Manage Foods
- [x] Manage Recipes
- [x] Manage Meals
- [x] Manage Vitals
- [x] Manage Goals
- [x] Manage Water Consumption
- [x] Manage Figure Measurements
- [x] Vitals Reports
- [ ] Manage Figure Goals
- [ ] OCR Scan Food Labels
- [ ] Barcode Scan Foods
- [ ] See what you can eat based on food goals
- [ ] Manage Cardio/Strength Excercises
- [ ] Parse Recipes from sites
