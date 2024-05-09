# Trade Data Reconciliation

This project involves the reconciliation of trade data for a client transitioning trading systems. The provided data contains trade information including quantities, prices, and net proceeds. The goal is to ensure the accuracy of the reported data by identifying and correcting discrepancies.

## Data Sample

The sample provided is as follows:

| TradeDate | Trader     | Security | Counterparty | Side | TradeQty | TradeCcy | UnitPrice | GrossProceeds | TotalCommission | NetProceeds |
|-----------|------------|----------|--------------|------|----------|----------|-----------|---------------|-----------------|-------------|
| 2/8/2011  | John Smith | AAPL     | GSEQ         | BUY  | 2,000    | USD      | 355.35    | -71,070,000   | -100            | -710,800    |
| 2/8/2011  | John Smith | AAPL     | GSEQ         | BUY  | 2,000    | USD      | 355.45    | -710,900      | -100            | -711,000    |
| 2/11/2011 | jsmith     | AAPL     | GSEQ         | SELL | 2,000    | USD      | 356.80    | 713,600       | 100             | 713,500     |

## Issues Identified

### Data Formatting and Calculation Issues
1. **TradeQty Formatting:**
   - The `TradeQty` column contains values with spaces.

2. **GrossProceeds and NetProceeds:**
   - The `GrossProceeds` for the first trade appears incorrect due to an extra zero and a sign error.
   - The `NetProceeds` for all trades contain errors due to incorrect calculations.

### Discrepancies
1. **GrossProceeds in the first trade entry:**
   - Incorrect value reported as -71,070,000.
2. **NetProceeds for the first and second trade entries:**
   - Incorrect due to wrong GrossProceeds or incorrect calculations.
3. **NetProceeds for the sell entry:**
   - Incorrect due to wrong calculations.

## Corrections

### Correct GrossProceeds
1. Correct the GrossProceeds for the first trade from -71,070,000 to 710,700.

### Recalculate NetProceeds
1. For the first trade (2/8/2011):
   - Correct NetProceeds should be: 710,700 - 100 = 710,600.
2. For the second trade (2/8/2011):
   - Correct NetProceeds should be: 710,900 - 100 = 710,800.
3. For the sell trade (2/11/2011):
   - Correct NetProceeds should be: 713,600 + 100 = 713,700.

## Corrected Data

| TradeDate | Trader     | Security | Counterparty | Side | TradeQty | TradeCcy | UnitPrice | GrossProceeds | TotalCommission | NetProceeds |
|-----------|------------|----------|--------------|------|----------|----------|-----------|---------------|-----------------|-------------|
| 2/8/2011  | John Smith | AAPL     | GSEQ         | BUY  | 2000     | USD      | 355.35    | 710,700       | -100            | 710,600     |
| 2/8/2011  | John Smith | AAPL     | GSEQ         | BUY  | 2000     | USD      | 355.45    | 710,900       | -100            | 710,800     |
| 2/11/2011 | jsmith     | AAPL     | GSEQ         | SELL | 2000     | USD      | 356.80    | 713,600       | 100             | 713,700     |

## Workflow and Calculations

1. **Verify TradeQty:**
   - Ensure all quantities are correctly formatted without spaces.

2. **Validate and Correct GrossProceeds:**
   - For each trade, recalculate GrossProceeds as `TradeQty * UnitPrice`.

3. **Validate and Correct NetProceeds:**
   - Apply the formula `NetProceeds = GrossProceeds - TotalCommission` for buys and `NetProceeds = GrossProceeds + TotalCommission` for sells.

4. **Document Changes:**
   - Note the discrepancies and corrections for each trade.

By following these steps and correcting the values, the data is now accurate and consistent.

