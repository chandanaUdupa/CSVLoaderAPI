-- Script Date: 9/24/2021 7:22 PM  - ErikEJ.SqlCeScripting version 3.5.2.90
CREATE TABLE [Product] (
  [Key] TEXT NOT NULL
, [ArtikelCode] TEXT NOT NULL
, [ColorCode] TEXT NOT NULL
, [Description] TEXT NOT NULL
, [Price] INTEGER NOT NULL
, [DiscountPrice] INTEGER NOT NULL
, [DeliveredIn] TEXT NOT NULL
, [Q1] TEXT NOT NULL
, [Size] INTEGER NOT NULL
, [Color] TEXT NOT NULL
, CONSTRAINT [PK_Product] PRIMARY KEY ([Key])
);
