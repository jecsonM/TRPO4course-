    INSERT INTO "MachineServiceDBScheme".RequestStates
    VALUES 
    (DEFAULT,'Отклонена'),
    (DEFAULT,'Создана'),
    (DEFAULT,'На рассмотрении'),
    (DEFAULT,'Сформирован заказ');

    INSERT INTO "MachineServiceDBScheme".Roles
    VALUES 
    (DEFAULT,'Мастер'),
    (DEFAULT,'Бухгалтер'),
    (DEFAULT,'Директор'),
    (DEFAULT,'Системный администратор');

    INSERT INTO "MachineServiceDBScheme".OrderStates
    VALUES 
    (DEFAULT,'Отменён'),
    (DEFAULT,'Сформирован'),
    (DEFAULT,'На перерассмотрении'),
    (DEFAULT,'Одобрен'),
    (DEFAULT,'Завершён');

    INSERT INTO "MachineServiceDBScheme".Clients
    VALUES (DEFAULT,'ООО Станко строй', 'Вайтенко Константин Игоревич', '79308551200', 'stankoStroy@mail.ru', 
    'г. Санкт Петрбург ул. Пушкина д.26 к2', '123456789101', '772501001', NULL),
    (DEFAULT,'ООО Консерв завод', 'Морозов Иван Олегович', '78885553300', 'konservZavod@mail.ru', 
    'г. Санкт Петрбург ул. Морозова д.6', '123456111111', '772501001', NULL),
    (DEFAULT,'АО РоссРуда', 'Козлов Виктор Игоревич', '76661235500', 'rossRuda@mail.ru', 
    'г. Москва ул. Поликарпова д.1 ', '444455559101', '772501001', NULL);

    INSERT INTO "MachineServiceDBScheme".Requests
    VALUES
    (DEFAULT, 1,'2026-01-25 16:30:00.000 +01:00','г. Санкт Петрбург ул. Пушкина д.26 к2'),
    (DEFAULT, 2,'2026-01-29 14:15:00.000 +01:00','г. Санкт Петрбург ул. Морозова д.6'),
    (DEFAULT, 1,'2026-01-29 14:30:00.000 +01:00','г. Санкт Петрбург ул. Пушкина д.26 к2');

    INSERT INTO "MachineServiceDBScheme".Machines
    VALUES
    (DEFAULT,1, 'A125B124', 'IRONMAC IT-560', NULL),
    (DEFAULT,1, 'A15CF244', 'IRONMAC IT-560', NULL),
    (DEFAULT,2, '12345678CCBBDD', 'КМТ KE50/1000', NULL);

    INSERT INTO "MachineServiceDBScheme".Staff
    VALUES (DEFAULT, 4, 'sysAdmin', NULL);


    INSERT INTO "MachineServiceDBScheme".Orders
    VALUES
    (DEFAULT,1,'2026-01-26 16:30:00.000 +01:00'),
    (DEFAULT,2,'2026-01-30 16:30:00.000 +01:00'),
    (DEFAULT,3,'2026-01-30 16:30:00.000 +01:00');

    INSERT INTO "MachineServiceDBScheme".MachineServices
    VALUES
    (DEFAULT,1, 'Замена гаек'),
    (DEFAULT,1, 'Натяжка ремней'),
    (DEFAULT,1, 'Удаление ржавчины');

    INSERT INTO "MachineServiceDBScheme".RelevantOrderStates
    VALUES
    (DEFAULT,1, 2, '2026-01-27 16:30:00.000 +01:00'),
    (DEFAULT,1, 4, '2026-01-27 16:30:00.000 +01:00'),
    (DEFAULT,2, 2, '2026-01-27 16:30:00.000 +01:00');


    INSERT INTO "MachineServiceDBScheme".MachinesInOrder
    VALUES
    (1, 1),
    (1, 2),
    (2, 3);

    INSERT INTO "MachineServiceDBScheme".ServiceProvisions
    VALUES 
    (1,1,1, 2),
    (1,2,1, 3),
    (2,3,1, 5);

    INSERT INTO "MachineServiceDBScheme".RelevantRequestStates
    VALUES 
    (1, 1, '2026-01-25 16:30:00.000 +01:00'),
    (1, 2, '2026-01-26 16:30:00.000 +01:00'),
    (1, 3, '2026-01-27 16:30:00.000 +01:00');

    INSERT INTO "MachineServiceDBScheme".RelevantCosts
    VALUES 
    (DEFAULT,1, 1, 250.55,'2026-01-10 16:30:00.000 +01:00'),
    (DEFAULT,2, 1, 600.28,'2026-01-10 16:32:00.000 +01:00'),
    (DEFAULT,3, 1, 2000.00,'2026-01-10 16:34:00.000 +01:00');
