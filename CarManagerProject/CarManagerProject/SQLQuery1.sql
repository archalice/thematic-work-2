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

INSERT INTO Car_Brands (brand_name, country, foundation_year)
VALUES
('Acura', 'Japan', 1986),
('Audi', 'Germany', 1909),
('Aston Martin', 'UK', 1913),
('Ferrari', 'Italy', 1939);
SELECT * FROM Car_Brands;


INSERT INTO Car_Models (brand_id, model_name, production_year, price, discount_percent, car_photo_img)
VALUES
(1, 'NSX', 2022, 170000, 10, 'Images/Acura-nsx.jpg'),
(1, 'MDX', 2023, 60000, 5, 'Images/acura-mdx.jpg'),

(2, 'RS7', 2023, 125000, 8, 'Images/audi-rs7.jpg'),
(2, 'Q8', 2022, 90000, 7, 'Images/audi-q8.jpg'),

(3, 'DB12', 2024, 250000, 6, 'Images/db12.jpg'),
(3, 'Vantage', 2023, 200000, 9, 'Images/aston-vantage.jpg'),

(4, 'Roma', 2023, 250000, 10, 'Images/ferrari-roma.jpg'),
(4, 'SF90 Stradale', 2024, 520000, 12, 'Images/ferrari-sf90.jpg');

SELECT * FROM Car_Models;
