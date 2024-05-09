# ASP.NET 8 MVC Application for Security Price Analysis

## Overview
This ASP.NET 8 MVC application is designed to analyze the security price movements of two companies, Corning, Inc. (GLW US) and Nvidia Corporation, Inc. (NVDA US), for the period from January 1, 2015, to June 30, 2015. The application retrieves pricing data, performs various analyses, and provides visualizations to help users understand the price movements and significant spikes during this period.

## Features
- **Data Retrieval**: Fetches historical pricing data for the specified securities.
- **Price Analysis**:
  - Calculates and reports the minimum, maximum, and average closing prices for each security over the specified period.
  - Identifies and reports the most significant positive spike in the price, along with a possible explanation for the spike.
  - Calculates the return on investment (ROI) for 1,000 shares from January 1, 2015, to the date of any significant price spike for the chosen security.
- **Data Visualization**: Creates visualizations to display the price movements and highlight any abnormal price moves.

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/) or any preferred IDE for ASP.NET development
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

## How To Use 🔧

From your command line, first clone PortfolioBI:

```bash
# Clone this repository
$ git clone https://github.com/IvanG2020/PortfolioBI.git

# Go into the repository
$ cd PortfolioBI

# Remove current origin repository
$ git remote remove origin

# In the dictory of PortfolioBI you must add migrations
$ Add-Migrations [migration name]

# In the directory of PortfolioBI you must update after migrations
$ Update-Database

# In the directory of PortfolioBI you must upload CVS files with the history stock data. This data will be inserted into the MS SQL
$ These documents are located in Data/DataSource NVDA.CVS & GLW.CVS
```
