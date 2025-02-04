# Тестовое задание – Events Web-application

## Как запустить проект

1. Клонировать репозиторий

   ```shell
   git clone https://github.com/DenKorporation/Modsen_Events.git
   
2. Перейти в папку с docker-compose.yml

   ```shell
   cd modsen_events/docker
   
3. Запустить docker-compose.yml

   ```shell
   docker compose up -d

4. После запуска доступны:

   - swagger: http://localhost:5000/swagger/index.html
   - angular client: http://localhost:4200
   - Предустановленный админ
        - admin@example.com
        - Pass123$

---

## Вступительная задача для стажировки по направлению .NET

## Содержание

1. [Описание](#описание)
    1. [Функционал Web-приложения](#функционал-web-приложения)
    2. [Информация о событии](#информация-о-событии)
    3. [Информация об участнике события](#информация-об-участнике-события)
    4. [Разработка клиентской части приложения](#разработка-клиентской-части-приложения)
    5. [Требования к Web API](#требования-к-web-api)
    6. [Необходимые к использованию технологии](#необходимые-к-использованию-технологии)
    7. [Архитектура](#архитектура)

---

## Описание

Разработка web-приложения для работы с событиями, выполняется на .Net Core, с использованием EF Core. Для разработки
клиентской части приложения следует использовать Angular или React.

Должна прилагаться инструкция по запуску проекта.

---

## Функционал Web-приложения

### Работа с событиями:

1. Получение списка всех событий;
2. Получение определенного события по его Id;
3. Получение события по его названию;
4. Добавление нового события;
5. Изменение информации о существующем событии;
6. Удаление события;
7. Получение списка событий по определенным критериям (по дате, месту проведения, категории события);
8. Возможность добавления изображений к событиям и их хранение.

### Работа с участниками:

1. Регистрация участия пользователя в событии;
2. Получение списка участников события;
3. Получение определенного участника по его Id;
4. Отмена участия пользователя в событии;
5. *Отправка уведомлений участникам события об изменениях в событии (например, об изменении даты или места проведения) (
   будет плюсом).

---

## Информация о событии

1. Название;
2. Описание;
3. Дата и время проведения;
4. Место проведения;
5. Категория события;
6. Максимальное количество участников;
7. Список участников;
8. Изображение.

---

## Информация об участнике события

1. Имя;
2. Фамилия;
3. Дата рождения;
4. Дата регистрации на событие;
5. Email.

---

## Разработка клиентской части приложения

1. Реализация страницы аутентификации/регистрации пользователей;
2. Реализация страницы отображения списка событий. Если все места на событие уже заняты, отображать информацию о том,
   что свободных мест нет;
3. Реализация страницы с подробной информацией о событии;
4. Реализация страницы регистрации на событие;
5. Реализация страницы для просмотра событий, участником которых пользователь является;
6. Реализация пагинации, поиска по названию/дате события, фильтрации по категории/месту проведения;
7. Для пользователей, обладающих правами администратора, должен быть предусмотрен раздел для управления списком событий,
   где они могут создавать, редактировать и удалять события.

---

## Требования к Web API

1. Реализация policy-based авторизации с использованием refresh и jwt access токенов;
2. Внедрение паттерна репозиторий и Unit of Work;
3. Разработка middleware для глобальной обработки исключений;
4. Реализация пагинации;
5. Обеспечение покрытия репозиториев и сервисов unit-тестами;
6. *Внедрение кеширования изображений (будет плюсом).

---

## Необходимые к использованию технологии

1. .Net 5.0+;
2. Entity Framework Core;
3. MS SQL / PostgreSQL or any other;
4. AutoMapper / Mapster or any other;
5. FluentValidation;
6. Authentication via jwt access & refresh token (ex.: IdentityServer4);
7. Swagger;
8. EF Fluent API;
9. Angular/React;
10. xUnit/nUnit.

---

## Архитектура

Приложение должно быть разработано на чистой архитектуре.
