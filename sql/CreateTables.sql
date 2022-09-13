drop table if exists Patients;
create table Patients (
  id int identity(1,1) not null,
  firstName nvarchar(max) not null,
  lastName nvarchar(max) not null,
  birthday date not null,
  gender nvarchar(100) not null,
  CONSTRAINT [PK_Patients] PRIMARY KEY CLUSTERED ([ID] ASC)
)

-- insert into Patients(firstName, lastName, birthday, gender)
-- values('tom', 'haumersen', '2010-01-01','m');

-- select * from Patients
