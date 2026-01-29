CREATE TABLE RequestStates 
(
request_state_id int Identity(1,1) PRIMARY KEY,
request_state_name varchar(30)
);

CREATE TABLE Roles 
(
role_id int Identity(1,1) PRIMARY KEY,
role_name varchar(30)
);

CREATE TABLE OrderStates 
(
order_state_id int Identity(1,1) PRIMARY KEY,
order_state_name varchar(30)
);

--Справочники

CREATE TABLE Clients 
(
client_id int Identity(1,1) PRIMARY KEY,
company_name varchar(max),
contact_person_fullname varchar(max),
contact_phone varchar(15),
email varchar(max),
main_address varchar(max),
INN varchar(12),
KPP varchar(9),
notes varchar(max)
);

CREATE TABLE Requests 
(
request_id int Identity(1,1) PRIMARY KEY,
client_id int,
creation_date datetimeoffset,
service_address varchar(max),

CONSTRAINT FK_Requests_Clients 
        FOREIGN KEY (client_id) 
        REFERENCES Clients (client_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);


CREATE TABLE Machines 
(
machine_id int Identity(1,1) PRIMARY KEY,
client_id int,
serial_number varchar(50),
model varchar(max),
masters_comment varchar(max),

CONSTRAINT FK_Machines_Clients 
        FOREIGN KEY (client_id) 
        REFERENCES Clients (client_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);


CREATE TABLE Staff 
(
staff_id int Identity(1,1) PRIMARY KEY,
role_id int,
login varchar(30),
password_hash varchar(max),

CONSTRAINT FK_Staff_Roles 
        FOREIGN KEY (role_id) 
        REFERENCES Roles (role_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);


CREATE TABLE Orders 
(
order_id int Identity(1,1) PRIMARY KEY,
request_id int,
creation_date datetimeoffset,

CONSTRAINT FK_Orders_Requests 
        FOREIGN KEY (request_id) 
        REFERENCES Requests (request_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);


CREATE TABLE MachineServices
(
service_id int Identity(1,1) PRIMARY KEY,
creators_id int,
machine_service_name varchar(max),

CONSTRAINT FK_MachineServices_Staff 
        FOREIGN KEY (creators_id) 
        REFERENCES Staff (staff_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);

CREATE TABLE RelevantOrderStates
(
relevant_order_state_id int Identity(1,1) PRIMARY KEY,
order_id int,
order_state_id int,
set_date datetimeoffset,

CONSTRAINT FK_RelevantOrderStates_OrderStates
        FOREIGN KEY (order_state_id) 
        REFERENCES OrderStates (order_state_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE,
CONSTRAINT FK_RelevantOrderStates_Orders 
        FOREIGN KEY (order_id) 
        REFERENCES Orders (order_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);

CREATE TABLE MachinesInOrder
(
order_id int,
machine_id int,

CONSTRAINT PK_Orders_Machines
        PRIMARY KEY(order_id, machine_id),
CONSTRAINT FK_MachinesInOrder_Orders
        FOREIGN KEY (order_id) 
        REFERENCES Orders (order_id)
        ON DELETE CASCADE,
CONSTRAINT FK_MachinesInOrder_Machines 
        FOREIGN KEY (machine_id) 
        REFERENCES Machines (machine_id)
        ON DELETE CASCADE
);

CREATE TABLE ServiceProvisions
(
order_id int,
service_id int,
masters_id int,
amount int

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
        ON DELETE SET NULL
);