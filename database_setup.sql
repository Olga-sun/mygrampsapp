use new_database
go

create table dbo.app_users
(
    id            int identity
        primary key,
    username      nvarchar(50)  not null
        unique,
    password_hash nvarchar(255) not null
)
go

create table dbo.document
(
    id         int identity
        primary key,
    kind       varchar(50) collate Ukrainian_CI_AS,
    title      nvarchar(200) collate Ukrainian_CI_AS,
    created_dt date,
    file_uri   nvarchar(200) collate Ukrainian_CI_AS,
    text       nvarchar(max) collate Ukrainian_CI_AS,
    user_id    int
        constraint FK_Document_User
            references dbo.app_users,
    file_data  varbinary(max)
)
go

create table dbo.event_type
(
    id    int identity
        primary key,
    code  varchar(50) collate Ukrainian_CI_AS
        unique,
    label nvarchar(100) collate Ukrainian_CI_AS
)
go

create table dbo.place
(
    id        int identity
        primary key,
    name      nvarchar(100) collate Ukrainian_CI_AS,
    type      varchar(50) collate Ukrainian_CI_AS,
    parent_id int
        constraint FK_place_parent
            references dbo.place,
    lat       float,
    lon       float
)
go

create table dbo.person
(
    id             int identity
        primary key,
    sex            char collate Ukrainian_CI_AS,
    birth_date     date,
    death_date     date,
    birth_place_id int
        constraint FK_person_birth_place
            references dbo.place,
    notes          nvarchar(max) collate Ukrainian_CI_AS,
    last_name      nvarchar(100) collate Ukrainian_CI_AS,
    first_name     nvarchar(100) collate Ukrainian_CI_AS,
    patronymic     nvarchar(100) collate Ukrainian_CI_AS,
    maiden_name    nvarchar(100) collate Ukrainian_CI_AS,
    user_id        int
        constraint FK_Person_User
            references dbo.app_users
)
go

create table dbo.family_union
(
    id          int identity
        primary key,
    partner1_id int
        constraint FK_union_partner1
            references dbo.person,
    partner2_id int
        constraint FK_union_partner2
            references dbo.person,
    type        varchar(20) collate Ukrainian_CI_AS,
    start_dt    date,
    end_dt      date,
    place_id    int
        constraint FK_union_place
            references dbo.place,
    source_id   int
        constraint FK_union_source
            references dbo.document,
    user_id     int
)
go

create table dbo.event
(
    id        int identity
        primary key,
    person_id int
        constraint FK_event_person
            references dbo.person,
    union_id  int
        constraint FK_event_union
            references dbo.family_union,
    type_id   int
        constraint FK_event_type
            references dbo.event_type,
    dt        date,
    place_id  int
        constraint FK_event_place
            references dbo.place,
    payload   nvarchar(max) collate Ukrainian_CI_AS,
    source_id int
        constraint FK_event_source
            references dbo.document,
    user_id   int
)
go

create table dbo.kinship
(
    id            int identity
        primary key,
    parent_id     int
        constraint FK_kinship_parent
            references dbo.person,
    child_id      int
        constraint FK_kinship_child
            references dbo.person,
    relation_type varchar(20) collate Ukrainian_CI_AS,
    source_id     int
        constraint FK_kinship_source
            references dbo.document,
    user_id       int
)
go

create table dbo.person_occupation
(
    id         int identity
        primary key,
    person_id  int
        constraint FK_occupation_person
            references dbo.person,
    occupation nvarchar(100) collate Ukrainian_CI_AS,
    from_dt    date,
    to_dt      date,
    place_id   int
        constraint FK_occupation_place
            references dbo.place,
    source_id  int
        constraint FK_occupation_source
            references dbo.document,
    user_id    int
)
go

