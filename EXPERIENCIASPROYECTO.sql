CREATE DATABASE PRYECTO_DE_EXPERIENCIA;
USE PRYECTO_DE_EXPERIENCIA;

CREATE TABLE Categoria (
    ID_CATEGORIA INT IDENTITY PRIMARY KEY,
    NOMBRE NVARCHAR(100) NOT NULL
);

INSERT INTO Categoria (NOMBRE)
VALUES
('Aventura'),
('Sandbox'),
('Deportes'),
('Party'),
('Acción'),
('Simulación'),
('Shooter'),
('Estrategia'),
('Carreras'),
('Terror'),
('Plataformas'),
('RPG');




CREATE OR ALTER PROCEDURE SP_PERFIL_USUARIO 
    @ID_USUARIO INT
AS
BEGIN
    -- 1. Obtener los datos principales del usuario
    SELECT
        ID_USUARIO,
        NOMBRE,
        CORREO,
        ROL
    FROM
        USUARIO
    WHERE
        ID_USUARIO = @ID_USUARIO;

    -- 2. Obtener el historial de compras
    SELECT
        C.ID_COMPRA,
        C.FECHA,
        J.NOMBRE AS NOMBRE_JUEGO,
        J.IMAGEN_URL,
        DC.PRECIO_UNITARIO
    FROM
        COMPRA C
    INNER JOIN
        DETALLE_COMPRA DC ON C.ID_COMPRA = DC.ID_COMPRA
    INNER JOIN
        JUEGO J ON DC.ID_JUEGO = J.ID_JUEGO
    WHERE
        C.ID_USUARIO = @ID_USUARIO
    ORDER BY
        C.FECHA DESC;

    -- 3. Obtener el total gastado
    SELECT
        SUM(TOTAL) AS TOTAL_GASTADO
    FROM
        COMPRA
    WHERE
        ID_USUARIO = @ID_USUARIO;

    -- 4. Obtener las recomendaciones
    SELECT
        R.ID_RECOMENDACION,
        J.NOMBRE,
        ISNULL(R.MOTIVO, 'Recomendado por tu compra') AS MOTIVO,
        J.IMAGEN_URL
    FROM
        RECOMENDACION R
    INNER JOIN
        JUEGO J ON R.ID_JUEGO = J.ID_JUEGO
    WHERE
        R.ID_USUARIO = @ID_USUARIO;
END


CREATE OR ALTER PROCEDURE sp_ObtenerJuegosParaVista
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        j.ID_JUEGO,
        j.NOMBRE,
        j.DESCRIPCION,
        j.PRECIO,
        j.ACTIVO,
        j.IMAGEN_URL,
        j.ID_CATEGORIA,
        c.NOMBRE AS NOMBRE_CATEGORIA
    FROM JUEGO j
    INNER JOIN CATEGORIA c ON j.ID_CATEGORIA = c.ID_CATEGORIA
    ORDER BY j.ID_JUEGO;
END
EXEC sp_ObtenerJuegosParaVista;


Select*from JUEGO

CREATE TABLE USUARIO (
    ID_USUARIO INT PRIMARY KEY IDENTITY,
    NOMBRE NVARCHAR(100),
    CORREO NVARCHAR(100) UNIQUE,
    CONTRASENA NVARCHAR(100),
    ROL NVARCHAR(50) NOT NULL DEFAULT 'Usuario'
);


CREATE TABLE JUEGO (
    ID_JUEGO INT PRIMARY KEY IDENTITY,
    NOMBRE NVARCHAR(100) NOT NULL,
    DESCRIPCION NVARCHAR(500),
    PRECIO DECIMAL(10,2),
    ID_CATEGORIA INT NOT NULL,
    IMAGEN_URL NVARCHAR(300),
    VIDEO_URL NVARCHAR(300),
    ACTIVO BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Juego_Categoria FOREIGN KEY (ID_CATEGORIA) REFERENCES Categoria(ID_CATEGORIA)
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
Select*from USUARIO;
CREATE OR Alter PROCEDURE SP_REGISTRAR_USUARIO
    @NOMBRE NVARCHAR(100),
    @CORREO NVARCHAR(100),
    @CONTRASENA NVARCHAR(100),
    @ROL NVARCHAR(50)
AS
BEGIN
    INSERT INTO USUARIO (NOMBRE, CORREO, CONTRASENA, ROL)
    VALUES (@NOMBRE, @CORREO, @CONTRASENA, @ROL);
END;


Select*from USUARIO

CREATE OR ALTER PROCEDURE SP_LISTAR_JUEGOS
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        j.ID_JUEGO,
        j.NOMBRE,
        j.DESCRIPCION,
        j.PRECIO,
        j.ACTIVO,
        j.IMAGEN_URL, 
		j.VIDEO_URL,
        c.NOMBRE AS NOMBRE_CATEGORIA,
		j.ID_CATEGORIA
    FROM JUEGO j
    INNER JOIN CATEGORIA c ON j.ID_CATEGORIA = c.ID_CATEGORIA
    WHERE j.ACTIVO = 1
    ORDER BY j.ID_JUEGO;
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
('Luis Gómez', 'luisgomez@yahoo.com', 'pass123'),
('Carlos Ruiz', 'carlos.ruiz@gmail.com', 'pass2025'), 
('Maria Lopez', 'maria@gmail.com', 'clave123'),
('Pedro Salas', 'pedro@gmail.com', 'pedrito'),
('Laura Diaz', 'laura@hotmail.com', 'laura456'),
('Erick Bravo', 'erickb@game.com', 'bravo1'),
('Karla Núñez', 'karla_nu@hotmail.com', 'karla22'),
('Leo Mendoza', 'leo.mz@gmail.com', 'leoPass');


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
    SELECT ID_JUEGO, NOMBRE, DESCRIPCION, PRECIO, ID_CATEGORIA, IMAGEN_URL
FROM JUEGO
WHERE NOMBRE LIKE '%' + @BUSQUEDA + '%'

END
Create or ALTER PROCEDURE SP_LOGIN
    @CORREO NVARCHAR(100),
    @CONTRASENA NVARCHAR(100)
AS
BEGIN
    SELECT ID_USUARIO, NOMBRE, CORREO, CONTRASENA, ROL
    FROM USUARIO
    WHERE CORREO = @CORREO AND CONTRASENA = @CONTRASENA;
END

INSERT INTO USUARIO (NOMBRE, CORREO, CONTRASENA, ROL)
VALUES ('Administrador', 'admin@juegos.com', 'admin123', 'Admin');

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


ALTER PROCEDURE SP_INSERTAR_JUEGO
    @NOMBRE NVARCHAR(100),
    @DESCRIPCION NVARCHAR(500),
    @PRECIO DECIMAL(10,2),
    @ID_CATEGORIA INT,
    @IMAGEN_URL NVARCHAR(300),
    @VIDEO_URL NVARCHAR(300),
    @ACTIVO BIT = 1   -- valor por defecto si no se manda
AS
BEGIN
    INSERT INTO JUEGO (NOMBRE, DESCRIPCION, PRECIO, ID_CATEGORIA, IMAGEN_URL, VIDEO_URL, ACTIVO)
    VALUES (@NOMBRE, @DESCRIPCION, @PRECIO, @ID_CATEGORIA, @IMAGEN_URL, @VIDEO_URL, @ACTIVO)
	SELECT SCOPE_IDENTITY() AS ID_JUEGO;
END




ALTER PROCEDURE SP_EDITAR_JUEGO
    @ID_JUEGO INT,
    @NOMBRE NVARCHAR(100),
    @DESCRIPCION NVARCHAR(500),
    @PRECIO DECIMAL(10,2),
    @ID_CATEGORIA INT,
    @IMAGEN_URL NVARCHAR(200) = NULL,
    @VIDEO_URL NVARCHAR(200) = NULL
AS
BEGIN
    UPDATE JUEGO
    SET 
        NOMBRE = @NOMBRE,
        DESCRIPCION = @DESCRIPCION,
        PRECIO = @PRECIO,
        ID_CATEGORIA = @ID_CATEGORIA,
        IMAGEN_URL = @IMAGEN_URL,
        VIDEO_URL = @VIDEO_URL
    WHERE ID_JUEGO = @ID_JUEGO;
END




-- Insert completo de juegos con imagen y video
INSERT INTO JUEGO (NOMBRE, DESCRIPCION, PRECIO, ID_CATEGORIA, IMAGEN_URL, VIDEO_URL)
VALUES
('Elden Ring', 'Juego de acción y rol en mundo abierto', 59.99, 11, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1245620/header.jpg', 
'https://www.youtube.com/watch?v=example1'),

('Minecraft', 'Explora mundos infinitos y construye cualquier cosa, desde casas sencillas hasta castillos imponentes. Juega en modo creativo o sobrevive en modo supervivencia.', 26.95, 2, 
'https://i.ytimg.com/vi_webp/ztNoBI0m_P0/maxresdefault.webp', 
'https://www.youtube.com/watch?v=Rla3FUlxJdE'),

('Minecraft Deluxe', 'La versión Deluxe de Minecraft ofrece una experiencia extendida con contenido exclusivo, ideal para jugadores creativos y fanáticos de la construcción sin límites.', 39.99, 2, 
'https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/software/switch/70070000016597/0a33bcaba879403460afe2ff2aafaaefeede964e0fc11a430f71077867cc87f1', 
'https://www.youtube.com/watch?v=MmB9b5njVbA'),

('FIFA 25', 'FIFA 23 trae consigo lo último en simulación de fútbol, con nuevas animaciones, físicas mejoradas y plantillas actualizadas para una experiencia más realista.', 49.99, 3, 
'https://media.tycsports.com/files/2022/07/19/454313/fifa-23-portada_1440x810_wmk.webp', 
'https://www.youtube.com/watch?v=o3V-GvvzjE4'),

('Among Us', 'Among Us es un juego multijugador donde tú y tus amigos deben descubrir al impostor entre la tripulación antes de que sea demasiado tarde.', 4.99, 4, 
'https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/software/switch/70010000036098/758ab0b61205081da2466386940752c70e0e5ea43bd39e8b9b13eaa455c69b7e', 
'https://www.youtube.com/watch?v=NSJ4cESNQfE'),

('Cyberpunk 2077', 'Cyberpunk 2077 te sumerge en una metrópolis futurista donde la tecnología y el caos gobiernan. Personaliza tu personaje y explora una narrativa compleja e inmersiva.', 29.99, 5, 
'https://variety.com/wp-content/uploads/2023/10/cyberpunk.jpeg?w=1000&h=667&crop=1', 
'https://www.youtube.com/watch?v=8X2kIfS6fb8'),

('Terraria', 'Terraria es una aventura en 2D donde puedes excavar, construir, luchar contra enemigos y explorar un mundo vasto lleno de secretos y objetos por descubrir.', 19.99, 2, 
'https://a.allegroimg.com/s512/1147c7/1ff47cd9410bb9750c92e989d367/Terraria-STEAM-NOWA-GRA-PELNA-POLSKA-WERSJA-PC-PL', 
'https://www.youtube.com/watch?v=w7uOhFTrrq0'),

('Red Dead Redemption 2', 'Red Dead Redemption 2 es un viaje cinematográfico a través del Salvaje Oeste. Vive como forajido, cazador o explorador en un mundo abierto impresionante.', 199.99, 1, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1174180/header.jpg', 
'https://www.youtube.com/watch?v=eaW0tYpxyp0'),

('Hades', 'Hades es un adictivo roguelike de acción donde juegas como el hijo de Hades intentando escapar del Inframundo, con poderes de los dioses del Olimpo.', 49.99, 5, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1145360/header.jpg', 
'https://www.youtube.com/watch?v=91t0ha9x0AE'),

('The Sims 4', 'En The Sims 4 puedes crear y controlar personas, construir sus casas y desarrollar sus historias en un mundo lleno de posibilidades y creatividad.', 89.90, 6, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1222670/header.jpg', 
'https://www.youtube.com/watch?v=7D-WpFCmvRA'),

('Valorant', 'Valorant es un shooter táctico por equipos donde cada agente tiene habilidades únicas. La estrategia y la puntería marcan la diferencia.', 0.00, 7, 
'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcShiabI3_h1JSwQiQFID7JJrBOL7Iogx4J0JA&s', 
'https://www.youtube.com/watch?v=e_E9W2vsRbQ'),

('Age of Empires IV', 'Age of Empires IV regresa con intensas batallas históricas y civilizaciones únicas. Construye imperios, gestiona recursos y domina a tus rivales.', 129.99, 8, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1466860/header.jpg', 
'https://www.youtube.com/watch?v=5TnynE3PuDE'),

('Forza Horizon 5', 'Forza Horizon 5 es una experiencia de carreras de mundo abierto en México. Disfruta de paisajes hermosos, coches potentes y eventos emocionantes.', 229.00, 9, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1551360/header.jpg', 
'https://www.youtube.com/watch?v=FYH9n37B7Yw'),

('Stardew Valley', 'Stardew Valley te permite escapar de la ciudad y comenzar una nueva vida en el campo. Cultiva, explora cuevas y haz amigos en el pueblo.', 34.99, 6, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/413150/header.jpg', 
'https://www.youtube.com/watch?v=ot7uXNQskhs'),

('Resident Evil Village', 'Resident Evil Village combina horror y acción en un entorno espeluznante lleno de misterios y enemigos aterradores. Enfrenta tus peores pesadillas.', 189.00, 10, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1196590/header.jpg', 
'https://www.youtube.com/watch?v=btFclZUXpzA'),

('Cuphead', 'Cuphead es un juego de plataformas con estética clásica de dibujos animados. Desafiante y único, cada jefe es una obra de arte en movimiento.', 45.00, 11, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/268910/header.jpg', 
'https://www.youtube.com/watch?v=NN-9SQXoi50'),

('Hollow Knight', 'Hollow Knight es una aventura metroidvania en un mundo subterráneo oscuro. Explora, mejora tus habilidades y enfréntate a enemigos únicos.', 55.00, 1, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/367520/header.jpg', 
'https://www.youtube.com/watch?v=UAO2urG23S4'),

('Left 4 Dead 2', 'Shooter cooperativo de supervivencia contra hordas de zombis.', 29.99, 7, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/550/header.jpg', 
'https://www.youtube.com/watch?v=PHm4lLHngwI');



Select*from JUEGO

INSERT INTO JUEGO (NOMBRE, DESCRIPCION, PRECIO, ID_CATEGORIA, IMAGEN_URL, VIDEO_URL)
VALUES
('God of War', 'Aventura épica de Kratos en la mitología nórdica.', 59.99, 1, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1593500/header.jpg', 
'https://www.youtube.com/watch?v=K0u_kAWLJOA'),

('Sekiro: Shadows Die Twice', 'Acción y sigilo con combates desafiantes en el Japón feudal.', 49.99, 5, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/814380/header.jpg', 
'https://www.youtube.com/watch?v=rXMX4YJ7Lks'),

('Animal Crossing: New Horizons', 'Simulación de vida en una isla paradisíaca.', 59.99, 6, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1050000/header.jpg', 
'https://www.youtube.com/watch?v=_3YNL0OWio0'),

('Call of Duty: Modern Warfare II', 'Shooter en primera persona con campaña y multijugador.', 69.99, 7, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1938090/header.jpg', 
'https://www.youtube.com/watch?v=i3IsLrPeZG8'),

('Assassin’s Creed Valhalla', 'Aventura y RPG en la era de los vikingos.', 59.99, 1, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1054500/header.jpg', 
'https://www.youtube.com/watch?v=eARa4PZn_aE'),

('The Witcher 3: Wild Hunt', 'RPG de acción en un mundo abierto lleno de monstruos y magia.', 39.99, 11, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/292030/header.jpg', 
'https://www.youtube.com/watch?v=c0i88t0Kacs'),

('Overwatch 2', 'Shooter en equipo con héroes únicos y habilidades especiales.', 39.99, 7, 
'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQF-FO8XcO5Y_n2zdntktGjs5M6Wd2cfUZrDA&s', 
'https://www.youtube.com/watch?v=FqnKB22pOC0'),

('Genshin Impact', 'RPG de acción en mundo abierto con elementos de exploración y gacha.', 0.00, 11, 
'https://fastcdn.hoyoverse.com/content-v2/plat/124031/5d2ba4371115d26de4c574b28311aed8_1088324040958400144.jpeg', 
'https://www.youtube.com/watch?v=HLUY1nICQRY'),

('Rocket League', 'Fútbol con coches acrobáticos y competitivo.', 19.99, 9, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/252950/header.jpg', 
'https://www.youtube.com/watch?v=SgSX3gOrj60'),

('Dead by Daylight', 'Survival horror multijugador donde un asesino persigue a los jugadores.', 29.99, 10, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/381210/header.jpg', 
'https://www.youtube.com/watch?v=JGhIXLO3ul8'),

('Fall Guys: Ultimate Knockout', 'Party game de obstáculos y competencias locas.', 14.99, 4, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1097150/header.jpg', 
'https://www.youtube.com/watch?v=AyADwdiW7rQ'),

('Diablo IV', 'RPG de acción con mazmorras, loot y combates épicos.', 69.99, 11, 
'https://blz-contentstack-images.akamaized.net/v3/assets/blt77f4425de611b362/blt976b08da1cf9e58b/66df937c6a434d1da6691106/d4-edition_standard-edition_960.webp', 
'https://www.youtube.com/watch?v=Ro26B394ZBM'),

('Persona 5 Royal', 'RPG japonés con historia profunda y estilo visual único.', 49.99, 11, 
'https://assets.nintendo.com/image/upload/c_fill,w_1200/q_auto:best/f_auto/dpr_2.0/ncom/software/switch/70010000043147/684bd8b00abcbf6dd122727a27c01a337f667bef825f4f4662efad9854b72fd4', 
'https://www.youtube.com/watch?v=SKpSpvFCZRw'),

('For Honor', 'Acción y estrategia medieval en combates multijugador.', 29.99, 5, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/304390/header.jpg', 
'https://www.youtube.com/watch?v=zFUymXnQ5z8'),

('Splatoon 3', 'Shooter colorido y competitivo con modos únicos de juego.', 59.99, 7, 
'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTwfouGvCi44Lr5SUxoBlX86_Mg9afRJCyyJg&s', 
'https://www.youtube.com/watch?v=RPwwXvafJBY'),

('Monster Hunter Rise', 'Acción y caza de monstruos en entornos épicos.', 59.99, 5, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/1446780/header.jpg', 
'https://www.youtube.com/watch?v=a6C5lH5b-f4'),

('Dark Souls III', 'RPG de acción con combates desafiantes y mundo oscuro.', 49.99, 11, 
'https://cdn.cloudflare.steamstatic.com/steam/apps/374320/header.jpg', 
'https://www.youtube.com/watch?v=cWBwFhUv1-8');
