CREATE TABLE Car_Brands (
    brand_id INT IDENTITY(1,1) PRIMARY KEY,
    brand_name NVARCHAR(100) NOT NULL,
    country NVARCHAR(100),
    foundation_year INT
);

CREATE TABLE Car_Models (
    model_id INT IDENTITY(1,1) PRIMARY KEY,
    brand_id INT NOT NULL,
    model_name NVARCHAR(100) NOT NULL,
    production_year INT,
    price DECIMAL(10,2) NOT NULL,
    discount_percent DECIMAL(5,2) NOT NULL,
    discounted_price AS (price - (price * discount_percent / 100)),
    car_photo_img NVARCHAR(255),
    CONSTRAINT FK_Car_Models_Car_Brands
        FOREIGN KEY (brand_id) REFERENCES Car_Brands(brand_id)
);

DELETE FROM Car_Models;
DELETE FROM Car_Brands;

DBCC CHECKIDENT ('Car_Models', RESEED, 0);
DBCC CHECKIDENT ('Car_Brands', RESEED, 0);

INSERT INTO Car_Brands (brand_name, country, foundation_year)
VALUES
('Acura', 'Japan', 1986),
('Audi', 'Germany', 1909),
('Aston Martin', 'UK', 1913),
('Ferrari', 'Italy', 1939),
('Bugatti', 'France', 1909);

SELECT * FROM Car_Brands;


INSERT INTO Car_Models (brand_id, model_name, production_year, price, discount_percent, car_photo_img)
VALUES
(1, 'NSX', 2022, 170000, 10, 'Images/Acura-nsx.jpg'),
(1, 'MDX', 2023, 60000, 5, 'Images/acura-mdx.jpg'),
(1, 'Integra', 2023, 35000, 7, 'Images/acura-integra.jpg'),
(1, 'TLX', 2022, 45000, 6, 'Images/acura-tlx.jpg'),
(1, 'RDX', 2023, 50000, 8, 'Images/acura-rdx.jpg'),

(2, 'RS7', 2023, 125000, 8, 'Images/audi-rs7.jpg'),
(2, 'Q8', 2022, 90000, 7, 'Images/audi-q8.jpg'),
(2, 'e-tron GT', 2023, 140000, 6, 'Images/audi-etron.jpg'),
(2, 'A8', 2022, 90000, 5, 'Images/audi-a8.jpg'),
(2, 'RS5', 2023, 80000, 7, 'Images/audi-rs5.jpg'),

(3, 'DB12', 2024, 250000, 6, 'Images/db12.jpg'),
(3, 'Vantage', 2023, 200000, 9, 'Images/aston-vantage.jpg'),
(3, 'DBX', 2023, 180000, 8, 'Images/aston-dbx.jpg'),
(3, 'Valhalla', 2024, 800000, 5, 'Images/aston-valhalla.jpg'),
(3, 'DB11', 2022, 220000, 7, 'Images/aston-db11.jpg'),

(4, 'Roma', 2023, 250000, 10, 'Images/ferrari-roma.jpg'),
(4, 'SF90 Stradale', 2024, 520000, 12, 'Images/ferrari-sf90.jpg'),
(4, 'J50', 2023, 340000, 9, 'Images/ferrari-j50.jpg'),
(4, 'F8 Tributo', 2022, 330000, 8, 'Images/ferrari-f8.jpg'),
(4, '488 Pista', 2022, 400000, 6, 'Images/ferrari-488.jpg'),

(5, 'Chiron', 2022, 3000000, 5, 'Images/bugatti-chiron.jpg'),
(5, 'Mistral', 2024, 5000000, 7, 'Images/bugatti-mistral.jpg'),
(5, 'Bolide', 2024, 4300000, 6, 'Images/bugatti-bolide.jpg'),
(5, 'Divo', 2021, 5800000, 8, 'Images/bugatti-divo.jpg'),
(5, 'Centodieci', 2022, 9000000, 5, 'Images/bugatti-centodieci.jpg');

SELECT * FROM Car_Models;
