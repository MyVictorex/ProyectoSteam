CREATE DATABASE PRYECTO_DE_EXPERIENCIA;
USE PRYECTO_DE_EXPERIENCIA;

CREATE TABLE USUARIO (
    ID_USUARIO INT PRIMARY KEY IDENTITY,
    NOMBRE NVARCHAR(100),
    CORREO NVARCHAR(100) UNIQUE,
    CONTRASENA NVARCHAR(100)
);

-- Agregar columna ROL con valor por defecto 'Usuario'
ALTER TABLE USUARIO
ADD ROL NVARCHAR(50) NOT NULL DEFAULT 'Usuario';

CREATE TABLE JUEGO (
    ID_JUEGO INT PRIMARY KEY IDENTITY,
    NOMBRE NVARCHAR(100),
    DESCRIPCION NVARCHAR(500),
    PRECIO DECIMAL(10,2),
    CATEGORIA NVARCHAR(50)
);
CREATE TABLE COMPRA (
    ID_COMPRA INT PRIMARY KEY IDENTITY,
    ID_USUARIO INT,
    FECHA DATETIME DEFAULT GETDATE(),
    TOTAL DECIMAL(10,2),
    FOREIGN KEY (ID_USUARIO) REFERENCES USUARIO(ID_USUARIO)
);
CREATE TABLE DETALLE_COMPRA (
    ID_DETALLE INT PRIMARY KEY IDENTITY,
    ID_COMPRA INT,
    ID_JUEGO INT,
    PRECIO_UNITARIO DECIMAL(10,2),
    FOREIGN KEY (ID_COMPRA) REFERENCES COMPRA(ID_COMPRA),
    FOREIGN KEY (ID_JUEGO) REFERENCES JUEGO(ID_JUEGO)
);
CREATE TABLE RECOMENDACION (
    ID_RECOMENDACION INT PRIMARY KEY IDENTITY,
    ID_USUARIO INT,
    ID_JUEGO INT,
    MOTIVO NVARCHAR(200),
    FOREIGN KEY (ID_USUARIO) REFERENCES USUARIO(ID_USUARIO),
    FOREIGN KEY (ID_JUEGO) REFERENCES JUEGO(ID_JUEGO)
);

CREATE PROCEDURE SP_REGISTRAR_USUARIO
    @NOMBRE NVARCHAR(100),
    @CORREO NVARCHAR(100),
    @CONTRASENA NVARCHAR(100)
AS
BEGIN
    INSERT INTO USUARIO (NOMBRE, CORREO, CONTRASENA)
    VALUES (@NOMBRE, @CORREO, @CONTRASENA)
END

Select*from USUARIO

ALTER PROCEDURE SP_LISTAR_JUEGOS
AS
BEGIN
    SELECT ID_JUEGO, NOMBRE, DESCRIPCION, PRECIO, CATEGORIA
    FROM JUEGO
    WHERE ACTIVO = 1;
END

CREATE PROCEDURE SP_REGISTRAR_COMPRA
    @ID_USUARIO INT,
    @TOTAL DECIMAL(10,2),
    @ID_COMPRA INT OUTPUT
AS
BEGIN
    INSERT INTO COMPRA (ID_USUARIO, TOTAL) 
    VALUES (@ID_USUARIO, @TOTAL);

    SET @ID_COMPRA = SCOPE_IDENTITY();
END

CREATE PROCEDURE SP_REGISTRAR_DETALLE_COMPRA
    @ID_COMPRA INT,
    @ID_JUEGO INT,
    @PRECIO_UNITARIO DECIMAL(10,2)
AS
BEGIN
    INSERT INTO DETALLE_COMPRA (ID_COMPRA, ID_JUEGO, PRECIO_UNITARIO)
    VALUES (@ID_COMPRA, @ID_JUEGO, @PRECIO_UNITARIO);
END

CREATE OR ALTER PROCEDURE SP_HISTORIAL_USUARIO
    @ID_USUARIO INT
AS
BEGIN
    SELECT 
        C.ID_COMPRA, 
        C.FECHA, 
        J.NOMBRE AS NOMBRE_JUEGO, 
        J.IMAGEN_URL,
        DC.PRECIO_UNITARIO
    FROM COMPRA C
    INNER JOIN DETALLE_COMPRA DC ON C.ID_COMPRA = DC.ID_COMPRA
    INNER JOIN JUEGO J ON DC.ID_JUEGO = J.ID_JUEGO
    WHERE C.ID_USUARIO = @ID_USUARIO
    ORDER BY C.FECHA DESC
END

INSERT INTO USUARIO (NOMBRE, CORREO, CONTRASENA)
VALUES
('Juan Pérez', 'juanperez@gmail.com', '1234'),
('Ana Torres', 'ana.torres@hotmail.com', 'abcd'),
('Luis Gómez', 'luisgomez@yahoo.com', 'pass123');

INSERT INTO USUARIO (NOMBRE, CORREO, CONTRASENA)
VALUES
('Carlos Ruiz', 'carlos.ruiz@gmail.com', 'pass2025'), 
('Maria Lopez', 'maria@gmail.com', 'clave123'),
('Pedro Salas', 'pedro@gmail.com', 'pedrito'),
('Laura Diaz', 'laura@hotmail.com', 'laura456'),
('Erick Bravo', 'erickb@game.com', 'bravo1'),
('Karla Núñez', 'karla_nu@hotmail.com', 'karla22'),
('Leo Mendoza', 'leo.mz@gmail.com', 'leoPass');


INSERT INTO JUEGO (NOMBRE, DESCRIPCION, PRECIO, CATEGORIA)
VALUES
('Elden Ring', 'Juego de acción y rol en mundo abierto', 59.99, 'RPG'),
('Minecraft', 'Juego de construcción y aventura', 26.95, 'Sandbox'),
('FIFA 25', 'Simulador de fútbol con equipos reales', 49.99, 'Deportes'),
('Among Us', 'Juego multijugador de deducción social', 4.99, 'Party'),
('Cyberpunk 2077', 'Juego de rol futurista de mundo abierto', 29.99, 'Acción');

-- Compra hecha por Juan Pérez
INSERT INTO COMPRA (ID_USUARIO, TOTAL)
VALUES (1, 89.98);
-- Juan compró Elden Ring y Minecraft
INSERT INTO DETALLE_COMPRA (ID_COMPRA, ID_JUEGO, PRECIO_UNITARIO)
VALUES 
(1, 1, 59.99),
(1, 2, 26.95);
-- Recomendación para Juan basada en sus compras de RPG
INSERT INTO RECOMENDACION (ID_USUARIO, ID_JUEGO, MOTIVO)
VALUES 
(1, 5, 'Basado en tu interés en juegos de rol como Elden Ring');

-- 1. Registrar usuario nuevo
EXEC SP_REGISTRAR_USUARIO @NOMBRE='Carlos Ruiz', @CORREO='carlos@gmail.com', @CONTRASENA='pass2025';

-- 2. Listar todos los juegos
EXEC SP_LISTAR_JUEGOS;

-- 3. Registrar compra con salida del nuevo ID
DECLARE @NCOMPRA INT;
EXEC SP_REGISTRAR_COMPRA @ID_USUARIO=1, @TOTAL=59.99, @ID_COMPRA=@NCOMPRA OUTPUT;
SELECT @NCOMPRA AS NuevaCompraID;

-- 4. Registrar detalle compra
EXEC SP_REGISTRAR_DETALLE_COMPRA @ID_COMPRA=@NCOMPRA, @ID_JUEGO=3, @PRECIO_UNITARIO=59.99;

-- 5. Historial de usuario
EXEC SP_HISTORIAL_USUARIO @ID_USUARIO=1;

-- 6. Buscar juegos con palabra clave "RPG"
EXEC SP_BUSCAR_JUEGOS @BUSQUEDA='RPG';

-- 7. Login (verificar usuario)
EXEC SP_LOGIN @CORREO='juanperez@gmail.com', @CONTRASENA='1234';

-- 8. Total gastado por usuario
EXEC SP_TOTAL_GASTADO_USUARIO @ID_USUARIO=1;

-- 9. Ver recomendaciones de usuario
EXEC SP_VER_RECOMENDACIONES @ID_USUARIO=1;


CREATE OR ALTER PROCEDURE SP_BUSCAR_JUEGOS
    @BUSQUEDA NVARCHAR(100)
AS
BEGIN
    SELECT ID_JUEGO, NOMBRE, DESCRIPCION, PRECIO, CATEGORIA, IMAGEN_URL
FROM JUEGO
WHERE NOMBRE LIKE '%' + @BUSQUEDA + '%'

END
ALTER PROCEDURE SP_LOGIN
    @CORREO NVARCHAR(100),
    @CONTRASENA NVARCHAR(100)
AS
BEGIN
    SELECT ID_USUARIO, NOMBRE, CORREO, CONTRASENA, ROL
    FROM USUARIO
    WHERE CORREO = @CORREO AND CONTRASENA = @CONTRASENA;
END

CREATE PROCEDURE SP_TOTAL_GASTADO_USUARIO
    @ID_USUARIO INT
AS
BEGIN
    SELECT SUM(TOTAL) AS TOTAL_GASTADO
    FROM COMPRA
    WHERE ID_USUARIO = @ID_USUARIO
END
CREATE OR ALTER PROCEDURE SP_VER_RECOMENDACIONES
    @ID_USUARIO INT
AS
BEGIN
    SELECT 
        R.ID_RECOMENDACION, 
        J.NOMBRE,
        ISNULL(R.MOTIVO, 'Recomendado por tu compra'),
        J.IMAGEN_URL  -- ✅ agrega esto
    FROM RECOMENDACION R
    INNER JOIN JUEGO J ON R.ID_JUEGO = J.ID_JUEGO
    WHERE R.ID_USUARIO = @ID_USUARIO
END








ALTER TABLE JUEGO
ADD ACTIVO BIT NOT NULL DEFAULT 1;

CREATE PROCEDURE SP_DESACTIVAR_JUEGO
    @ID_JUEGO INT
AS
BEGIN
    UPDATE JUEGO
    SET ACTIVO = 0
    WHERE ID_JUEGO = @ID_JUEGO;
END
CREATE PROCEDURE SP_ACTIVAR_JUEGO
    @ID_JUEGO INT
AS
BEGIN
    UPDATE JUEGO
    SET ACTIVO = 1
    WHERE ID_JUEGO = @ID_JUEGO;
END

ALTER TABLE JUEGO
ADD IMAGEN_URL NVARCHAR(300);

ALTER PROCEDURE SP_INSERTAR_JUEGO
    @NOMBRE NVARCHAR(100),
    @DESCRIPCION NVARCHAR(500),
    @PRECIO DECIMAL(10,2),
    @CATEGORIA NVARCHAR(50),
    @IMAGEN_URL NVARCHAR(300),
    @VIDEO_URL NVARCHAR(300)
AS
BEGIN
    INSERT INTO JUEGO (NOMBRE, DESCRIPCION, PRECIO, CATEGORIA, IMAGEN_URL, VIDEO_URL)
    VALUES (@NOMBRE, @DESCRIPCION, @PRECIO, @CATEGORIA, @IMAGEN_URL, @VIDEO_URL)
END



CREATE PROCEDURE SP_EDITAR_JUEGO
    @ID_JUEGO INT,
    @NOMBRE NVARCHAR(100),
    @DESCRIPCION NVARCHAR(500),
    @PRECIO DECIMAL(10,2),
    @CATEGORIA NVARCHAR(50)
AS
BEGIN
    UPDATE JUEGO
    SET 
        NOMBRE = @NOMBRE,
        DESCRIPCION = @DESCRIPCION,
        PRECIO = @PRECIO,
        CATEGORIA = @CATEGORIA
    WHERE ID_JUEGO = @ID_JUEGO;
END

UPDATE JUEGO
SET IMAGEN_URL = 'https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/software/switch/70070000016597/0a33bcaba879403460afe2ff2aafaaefeede964e0fc11a430f71077867cc87f1'
WHERE NOMBRE = 'Minecraft Deluxe';

UPDATE JUEGO
SET IMAGEN_URL = 'https://a.allegroimg.com/s512/1147c7/1ff47cd9410bb9750c92e989d367/Terraria-STEAM-NOWA-GRA-PELNA-POLSKA-WERSJA-PC-PL'
WHERE NOMBRE = 'Terraria';

UPDATE JUEGO
SET IMAGEN_URL = 'https://i.ytimg.com/vi_webp/ztNoBI0m_P0/maxresdefault.webp'
WHERE NOMBRE = 'Minecraft';

UPDATE JUEGO
SET IMAGEN_URL = 'https://media.tycsports.com/files/2022/07/19/454313/fifa-23-portada_1440x810_wmk.webp'
WHERE NOMBRE = 'FIFA 25';

UPDATE JUEGO
SET IMAGEN_URL = 'https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/software/switch/70010000036098/758ab0b61205081da2466386940752c70e0e5ea43bd39e8b9b13eaa455c69b7e'
WHERE NOMBRE = 'Among Us';

UPDATE JUEGO
SET IMAGEN_URL = 'https://variety.com/wp-content/uploads/2023/10/cyberpunk.jpeg?w=1000&h=667&crop=1'
WHERE NOMBRE = 'Cyberpunk 2077';

SELECT * FROM JUEGO

INSERT INTO JUEGO (NOMBRE, DESCRIPCION, PRECIO, CATEGORIA, IMAGEN_URL)
VALUES
-- 1
('Red Dead Redemption 2',
 'Juego de acción-aventura en mundo abierto del viejo oeste.',
 199.99, 'Aventura',
 'https://cdn.cloudflare.steamstatic.com/steam/apps/1174180/header.jpg'),

-- 2
('Hades',
 'Juego de acción roguelike con temática mitológica griega.',
 49.99, 'Acción',
 'https://cdn.cloudflare.steamstatic.com/steam/apps/1145360/header.jpg'),

-- 3
('The Sims 4',
 'Simulación de vida donde puedes crear y controlar personas.',
 89.90, 'Simulación',
 'https://cdn.cloudflare.steamstatic.com/steam/apps/1222670/header.jpg'),

-- 4
('Valorant',
 'Shooter táctico en línea 5v5 con habilidades únicas por agente.',
 0.00, 'Shooter',
 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcShiabI3_h1JSwQiQFID7JJrBOL7Iogx4J0JA&s'),

-- 5
('Age of Empires IV',
 'Juego de estrategia en tiempo real ambientado en la historia.',
 129.99, 'Estrategia',
 'https://cdn.cloudflare.steamstatic.com/steam/apps/1466860/header.jpg'),

-- 6
('Forza Horizon 5',
 'Carreras de autos en mundo abierto ambientado en México.',
 229.00, 'Carreras',
 'https://cdn.cloudflare.steamstatic.com/steam/apps/1551360/header.jpg'),

-- 7
('Stardew Valley',
 'Simulador de granja con exploración y relaciones sociales.',
 34.99, 'Simulación',
 'https://cdn.cloudflare.steamstatic.com/steam/apps/413150/header.jpg'),

-- 8
('Resident Evil Village',
 'Survival horror con acción intensa en una aldea misteriosa.',
 189.00, 'Terror',
 'https://cdn.cloudflare.steamstatic.com/steam/apps/1196590/header.jpg'),

-- 9
('Cuphead',
 'Plataforma de acción con estilo de dibujos animados clásicos.',
 45.00, 'Plataformas',
 'https://cdn.cloudflare.steamstatic.com/steam/apps/268910/header.jpg'),

-- 10
('Hollow Knight',
 'Metroidvania oscuro con exploración profunda y combates.',
 55.00, 'Aventura',
 'https://cdn.cloudflare.steamstatic.com/steam/apps/367520/header.jpg');



 ALTER TABLE JUEGO
ADD VIDEO_URL NVARCHAR(300);


Select*from JUEGO

UPDATE JUEGO SET 
DESCRIPCION = 'La versión Deluxe de Minecraft ofrece una experiencia extendida con contenido exclusivo, ideal para jugadores creativos y fanáticos de la construcción sin límites.',
VIDEO_URL = 'https://www.youtube.com/watch?v=MmB9b5njVbA'
WHERE NOMBRE = 'Minecraft Deluxe';

UPDATE JUEGO SET 
DESCRIPCION = 'Explora mundos infinitos y construye cualquier cosa, desde casas sencillas hasta castillos imponentes. Juega en modo creativo o sobrevive en modo supervivencia.',
VIDEO_URL = 'https://www.youtube.com/watch?v=Rla3FUlxJdE'
WHERE NOMBRE = 'Minecraft';

UPDATE JUEGO SET 
DESCRIPCION = 'FIFA 23 trae consigo lo último en simulación de fútbol, con nuevas animaciones, físicas mejoradas y plantillas actualizadas para una experiencia más realista.',
VIDEO_URL = 'https://www.youtube.com/watch?v=o3V-GvvzjE4' -- ficticio, cambia si tienes uno real
WHERE NOMBRE = 'FIFA 25';

UPDATE JUEGO SET 
DESCRIPCION = 'Among Us es un juego multijugador donde tú y tus amigos deben descubrir al impostor entre la tripulación antes de que sea demasiado tarde.',
VIDEO_URL = 'https://www.youtube.com/watch?v=NSJ4cESNQfE'
WHERE NOMBRE = 'Among Us';

UPDATE JUEGO SET 
DESCRIPCION = 'Cyberpunk 2077 te sumerge en una metrópolis futurista donde la tecnología y el caos gobiernan. Personaliza tu personaje y explora una narrativa compleja e inmersiva.',
VIDEO_URL = 'https://www.youtube.com/watch?v=8X2kIfS6fb8'
WHERE NOMBRE = 'Cyberpunk 2077';

UPDATE JUEGO SET 
DESCRIPCION = 'Terraria es una aventura en 2D donde puedes excavar, construir, luchar contra enemigos y explorar un mundo vasto lleno de secretos y objetos por descubrir.',
VIDEO_URL = 'https://www.youtube.com/watch?v=w7uOhFTrrq0'
WHERE NOMBRE = 'Terraria';

UPDATE JUEGO SET 
DESCRIPCION = 'Red Dead Redemption 2 es un viaje cinematográfico a través del Salvaje Oeste. Vive como forajido, cazador o explorador en un mundo abierto impresionante.',
VIDEO_URL = 'https://www.youtube.com/watch?v=eaW0tYpxyp0'
WHERE NOMBRE = 'Red Dead Redemption 2';

UPDATE JUEGO SET 
DESCRIPCION = 'Hades es un adictivo roguelike de acción donde juegas como el hijo de Hades intentando escapar del Inframundo, con poderes de los dioses del Olimpo.',
VIDEO_URL = 'https://www.youtube.com/watch?v=91t0ha9x0AE'
WHERE NOMBRE = 'Hades';

UPDATE JUEGO SET 
DESCRIPCION = 'En The Sims 4 puedes crear y controlar personas, construir sus casas y desarrollar sus historias en un mundo lleno de posibilidades y creatividad.',
VIDEO_URL = 'https://www.youtube.com/watch?v=7D-WpFCmvRA'
WHERE NOMBRE = 'The Sims 4';

UPDATE JUEGO SET 
DESCRIPCION = 'Valorant es un shooter táctico por equipos donde cada agente tiene habilidades únicas. La estrategia y la puntería marcan la diferencia.',
VIDEO_URL = 'https://www.youtube.com/watch?v=e_E9W2vsRbQ'
WHERE NOMBRE = 'Valorant';

UPDATE JUEGO SET 
DESCRIPCION = 'Age of Empires IV regresa con intensas batallas históricas y civilizaciones únicas. Construye imperios, gestiona recursos y domina a tus rivales.',
VIDEO_URL = 'https://www.youtube.com/watch?v=5TnynE3PuDE'
WHERE NOMBRE = 'Age of Empires IV';

UPDATE JUEGO SET 
DESCRIPCION = 'Forza Horizon 5 es una experiencia de carreras de mundo abierto en México. Disfruta de paisajes hermosos, coches potentes y eventos emocionantes.',
VIDEO_URL = 'https://www.youtube.com/watch?v=FYH9n37B7Yw'
WHERE NOMBRE = 'Forza Horizon 5';

UPDATE JUEGO SET 
DESCRIPCION = 'Stardew Valley te permite escapar de la ciudad y comenzar una nueva vida en el campo. Cultiva, explora cuevas y haz amigos en el pueblo.',
VIDEO_URL = 'https://www.youtube.com/watch?v=ot7uXNQskhs'
WHERE NOMBRE = 'Stardew Valley';

UPDATE JUEGO SET 
DESCRIPCION = 'Resident Evil Village combina horror y acción en un entorno espeluznante lleno de misterios y enemigos aterradores. Enfrenta tus peores pesadillas.',
VIDEO_URL = 'https://www.youtube.com/watch?v=btFclZUXpzA'
WHERE NOMBRE = 'Resident Evil Village';

UPDATE JUEGO SET 
DESCRIPCION = 'Cuphead es un juego de plataformas con estética clásica de dibujos animados. Desafiante y único, cada jefe es una obra de arte en movimiento.',
VIDEO_URL = 'https://www.youtube.com/watch?v=NN-9SQXoi50'
WHERE NOMBRE = 'Cuphead';

UPDATE JUEGO SET 
DESCRIPCION = 'Hollow Knight es una aventura metroidvania en un mundo subterráneo oscuro. Explora, mejora tus habilidades y enfréntate a enemigos únicos.',
VIDEO_URL = 'https://www.youtube.com/watch?v=UAO2urG23S4'
WHERE NOMBRE = 'Hollow Knight';


UPDATE JUEGO SET 
VIDEO_URL = 'https://www.youtube.com/watch?v=PHm4lLHngwI'
WHERE NOMBRE = 'Left 4 dead 2';


