CREATE TABLE "MachineServiceDBScheme".RequestStates 
(
request_state_id int NOT NULL GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
request_state_name varchar(30) NOT NULL
);

CREATE TABLE "MachineServiceDBScheme".Roles 
(
role_id int NOT NULL GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
role_name varchar(30) NOT NULL
);

CREATE TABLE "MachineServiceDBScheme".OrderStates 
(
order_state_id int NOT NULL GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
order_state_name varchar(30) NOT NULL
);

--Справочники

CREATE TABLE "MachineServiceDBScheme".Clients 
(
client_id int NOT NULL GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
company_name varchar(256) NOT NULL,
contact_person_fullname varchar(256) NOT NULL,
contact_phone varchar(15) NOT NULL,
email varchar(256) NOT NULL,
main_address varchar(256) NOT NULL,
INN varchar(12) NOT NULL,
KPP varchar(9) NOT NULL,
notes varchar(256)
);

CREATE TABLE "MachineServiceDBScheme".Requests 
(
request_id int NOT NULL GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
client_id int NOT NULL,
creation_date TIMESTAMPTZ NOT NULL,
service_address varchar(256) NOT NULL,

CONSTRAINT FK_Requests_Clients 
        FOREIGN KEY (client_id) 
        REFERENCES "MachineServiceDBScheme".Clients (client_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);


CREATE TABLE "MachineServiceDBScheme".Machines 
(
machine_id int NOT NULL GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
client_id int NOT NULL,
serial_number varchar(50) NOT NULL,
model varchar(256) NOT NULL,
masters_comment varchar(256),

CONSTRAINT FK_Machines_Clients 
        FOREIGN KEY (client_id) 
        REFERENCES "MachineServiceDBScheme".Clients (client_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);


CREATE TABLE "MachineServiceDBScheme".Staff 
(
staff_id int NOT NULL GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
role_id int,
login  varchar(256) NOT NULL,
password_hash bytea NULL,

CONSTRAINT FK_Staff_Roles 
        FOREIGN KEY (role_id) 
        REFERENCES "MachineServiceDBScheme".Roles (role_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);


CREATE TABLE "MachineServiceDBScheme".Orders 
(
order_id  int NOT NULL GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
request_id int NOT NULL,
creation_date TIMESTAMPTZ NOT NULL,

CONSTRAINT FK_Orders_Requests 
        FOREIGN KEY (request_id) 
        REFERENCES "MachineServiceDBScheme".Requests (request_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);


CREATE TABLE "MachineServiceDBScheme".MachineServices
(
service_id int NOT NULL GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
creators_id int,
machine_service_name varchar(256) NOT NULL,

CONSTRAINT FK_MachineServices_Staff 
        FOREIGN KEY (creators_id) 
        REFERENCES "MachineServiceDBScheme".Staff (staff_id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);

CREATE TABLE "MachineServiceDBScheme".RelevantOrderStates
(
relevant_order_state_id  int NOT NULL GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
order_id int NOT NULL,
order_state_id int NOT NULL,
set_date TIMESTAMPTZ NOT NULL,

CONSTRAINT FK_RelevantOrderStates_OrderStates
        FOREIGN KEY (order_state_id) 
        REFERENCES "MachineServiceDBScheme".OrderStates (order_state_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
CONSTRAINT FK_RelevantOrderStates_Orders 
        FOREIGN KEY (order_id) 
        REFERENCES "MachineServiceDBScheme".Orders (order_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE "MachineServiceDBScheme".MachinesInOrder
(
order_id int NOT NULL,
machine_id int NOT NULL,

CONSTRAINT PK_Orders_Machines
        PRIMARY KEY(order_id, machine_id),
CONSTRAINT FK_MachinesInOrder_Orders
        FOREIGN KEY (order_id) 
        REFERENCES "MachineServiceDBScheme".Orders (order_id),
CONSTRAINT FK_MachinesInOrder_Machines 
        FOREIGN KEY (machine_id) 
        REFERENCES "MachineServiceDBScheme".Machines (machine_id)
);

CREATE TABLE "MachineServiceDBScheme".ServiceProvisions
(
order_id int NOT NULL,
service_id int NOT NULL,
masters_id int NOT NULL,
amount int NOT NULL,

CONSTRAINT PK_Orders_MachineServices
        PRIMARY KEY(order_id, service_id),
CONSTRAINT FK_ServiceProvisions_Orders
        FOREIGN KEY (order_id) 
        REFERENCES "MachineServiceDBScheme".Orders (order_id)
        ON DELETE CASCADE,
CONSTRAINT FK_ServiceProvisions_MachineServices 
        FOREIGN KEY (service_id) 
        REFERENCES "MachineServiceDBScheme".MachineServices (service_id)
        ON DELETE CASCADE,
CONSTRAINT FK_ServiceProvisions_Staff 
        FOREIGN KEY (masters_id) 
        REFERENCES "MachineServiceDBScheme".Staff (staff_id)
        ON DELETE CASCADE
);

CREATE TABLE "MachineServiceDBScheme".RelevantRequestStates 
(
relevant_request_state_id INT NOT NULL GENERATED ALWAYS AS IDENTITY,
request_id int NOT NULL,
request_state_id int NOT NULL,
set_date TIMESTAMPTZ NOT NULL,

CONSTRAINT FK_RelevantRequestStates_RequestStates 
        FOREIGN KEY (request_state_id) 
        REFERENCES "MachineServiceDBScheme".RequestStates (request_state_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
CONSTRAINT FK_RelevantRequestStates_Requests 
        FOREIGN KEY (request_id) 
        REFERENCES "MachineServiceDBScheme".Requests (request_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);

CREATE TABLE "MachineServiceDBScheme".RelevantCosts
(
relevantCostId INT NOT NULL GENERATED ALWAYS AS IDENTITY,
service_id int NOT NULL,
creators_id int NOT NULL,
relevant_cost money NOT NULL,
set_date TIMESTAMPTZ NOT NULL,

CONSTRAINT FK_RelevantCosts_Staff
        FOREIGN KEY (creators_id) 
        REFERENCES "MachineServiceDBScheme".Staff (staff_id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
CONSTRAINT FK_RelevantCosts_MachineServices 
        FOREIGN KEY (service_id) 
        REFERENCES "MachineServiceDBScheme".MachineServices (service_id)
);