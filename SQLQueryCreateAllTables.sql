CREATE TABLE RequestStates 
(
request_state_id int Identity(1,1) PRIMARY KEY NOT NULL,
request_state_name varchar(30) NOT NULL
);

CREATE TABLE Roles 
(
role_id int Identity(1,1) PRIMARY KEY NOT NULL,
role_name varchar(30) NOT NULL
);

CREATE TABLE OrderStates 
(
order_state_id int Identity(1,1) PRIMARY KEY NOT NULL,
order_state_name varchar(30) NOT NULL
);

--�����������

CREATE TABLE Clients 
(
client_id int Identity(1,1) PRIMARY KEY NOT NULL,
company_name varchar(max) NOT NULL,
contact_person_fullname varchar(max) NOT NULL,
contact_phone varchar(15) NOT NULL,
email varchar(max) NOT NULL,
main_address varchar(max) NOT NULL,
INN varchar(12) NOT NULL,
KPP varchar(9) NOT NULL,
notes varchar(max)
);

CREATE TABLE Requests 
(
request_id int Identity(1,1) PRIMARY KEY NOT NULL,
client_id int NOT NULL,
creation_date datetimeoffset NOT NULL,
service_address varchar(max) NOT NULL,

CONSTRAINT FK_Requests_Clients 
        FOREIGN KEY (client_id) 
        REFERENCES Clients (client_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);


CREATE TABLE Machines 
(
machine_id int Identity(1,1) PRIMARY KEY NOT NULL,
client_id int NOT NULL,
serial_number varchar(50) NOT NULL,
model varchar(max) NOT NULL,
masters_comment varchar(max),

CONSTRAINT FK_Machines_Clients 
        FOREIGN KEY (client_id) 
        REFERENCES Clients (client_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);


CREATE TABLE Staff 
(
staff_id int Identity(1,1) PRIMARY KEY NOT NULL,
role_id int,
login  varchar(max) NOT NULL,
password_hash varbinary(max) NULL,

CONSTRAINT FK_Staff_Roles 
        FOREIGN KEY (role_id) 
        REFERENCES Roles (role_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);


CREATE TABLE Orders 
(
order_id int Identity(1,1) PRIMARY KEY,
request_id int NOT NULL,
creation_date datetimeoffset NOT NULL,

CONSTRAINT FK_Orders_Requests 
        FOREIGN KEY (request_id) 
        REFERENCES Requests (request_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);


CREATE TABLE MachineServices
(
service_id int Identity(1,1) PRIMARY KEY,
creators_id int,
machine_service_name varchar(max) NOT NULL,

CONSTRAINT FK_MachineServices_Staff 
        FOREIGN KEY (creators_id) 
        REFERENCES Staff (staff_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);

CREATE TABLE RelevantOrderStates
(
relevant_order_state_id int Identity(1,1) PRIMARY KEY NOT NULL,
order_id int NOT NULL,
order_state_id int NOT NULL,
set_date datetimeoffset NOT NULL,

CONSTRAINT FK_RelevantOrderStates_OrderStates
        FOREIGN KEY (order_state_id) 
        REFERENCES OrderStates (order_state_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
CONSTRAINT FK_RelevantOrderStates_Orders 
        FOREIGN KEY (order_id) 
        REFERENCES Orders (order_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE MachinesInOrder
(
order_id int NOT NULL,
machine_id int NOT NULL,

CONSTRAINT PK_Orders_Machines
        PRIMARY KEY(order_id, machine_id),
CONSTRAINT FK_MachinesInOrder_Orders
        FOREIGN KEY (order_id) 
        REFERENCES Orders (order_id),
CONSTRAINT FK_MachinesInOrder_Machines 
        FOREIGN KEY (machine_id) 
        REFERENCES Machines (machine_id)
);

CREATE TABLE ServiceProvisions
(
order_id int NOT NULL,
service_id int NOT NULL,
masters_id int NOT NULL,
amount int NOT NULL

CONSTRAINT PK_Orders_MachineServices
        PRIMARY KEY(order_id, service_id),
CONSTRAINT FK_ServiceProvisions_Orders
        FOREIGN KEY (order_id) 
        REFERENCES Orders (order_id)
        ON DELETE CASCADE,
CONSTRAINT FK_ServiceProvisions_MachineServices 
        FOREIGN KEY (service_id) 
        REFERENCES MachineServices (service_id)
        ON DELETE CASCADE,
CONSTRAINT FK_ServiceProvisions_Staff 
        FOREIGN KEY (masters_id) 
        REFERENCES Staff (staff_id)
        ON DELETE CASCADE
);

CREATE TABLE RelevantRequestStates 
(
relevant_request_state_id  int Identity(1,1) PRIMARY KEY NOT NULL,
request_id int NOT NULL,
request_state_id int NOT NULL,
set_date datetimeoffset NOT NULL,

CONSTRAINT FK_RelevantRequestStates_RequestStates 
        FOREIGN KEY (request_state_id) 
        REFERENCES RequestStates (request_state_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
CONSTRAINT FK_RelevantRequestStates_Requests 
        FOREIGN KEY (request_id) 
        REFERENCES Requests (request_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE RelevantCosts
(
relevantCostId int Identity(1,1) PRIMARY KEY NOT NULL,
service_id int NOT NULL,
creators_id int NOT NULL,
relevant_cost money NOT NULL,
set_date datetimeoffset NOT NULL,

CONSTRAINT FK_RelevantCosts_Staff
        FOREIGN KEY (creators_id) 
        REFERENCES Staff (staff_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
CONSTRAINT FK_RelevantCosts_MachineServices 
        FOREIGN KEY (service_id) 
        REFERENCES MachineServices (service_id)
);