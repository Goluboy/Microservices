# Платформа онлайн образования
## **Функциональные требования**
### **Управление пользователями и аутентификация**
Платформа должна обеспечивать регистрацию, аутентификацию и авторизацию пользователей с поддержкой ролей (студенты, инструкторы, администраторы). JWT-токены, двухфакторную аутентификацию.

### **Управление курсами и образовательным контентом**
Платформа должна позволять создание, редактирование, сопровождение курсов с различным контентом (видео, файлы, тесты). Разбиение курсов по модулям и возможность задания вариаций одних и тех же заданий.

### **Система оценивания и тестирования**
Платформа должна поддерживать создание различных типов тестов (с множественный выбор, поле ввода для ответа, загрузка файлов), автоматическую проверку ответов и выставление оценок с возможностью ручной корректировки.

### **Платежная система и управление подписками**
Платформа должна обрабатывать платежи за курсы через различные способы оплаты, управлять подписками и автоматически предоставлять доступ к оплаченному контенту.

### **Система уведомлений**
Платформа должна отправлять уведомления через email о важных событиях (новые курсы, дедлайны, результаты тестов, изменения в оцененных баллах).

### **Аналитика и отчетность**
Платформа должна предоставлять детальную аналитику по прогрессу студентов, эффективности курсов и финансовым показателям для различных ролей пользователей.

## **Микросервисы**
### **User Management Service (Сервис управления пользователями)**
- Регистрация и управление учетными записями пользователей
- Аутентификация с JWT токенами и refresh токенами
- Авторизация на основе ролей
- Управление профилями и настройками пользователей

| HTTP метод | Путь запроса                 | Описание                       | Параметры запроса               | Ответ                |
| ---------- | ---------------------------- | ------------------------------ | ------------------------------- | -------------------- |
| GET        | /user?page=&pageSize=        | Получить всех пользователей    | page, pageSize                  | List\<UserResponse\> |
| GET        | /user/{id}                   | Получить пользователя по ID    | id: Guid                        | UserResponse         |
| GET        | /user/by-username/{username} | Получить пользователя по имени | username: string                | UserResponse         |
| GET        | /user/by-email/{email}       | Получить пользователя по email | email: string                   | UserResponse         |
| POST       | /user/register               | Создать нового пользователя    | CreateUserRequest               | UserResponse         |
| POST       | /user/login                  | Войти в аккаунт                | LoginRequest                    | UserResponse         |
| PUT        | /user                        | Обновить пользователя          | id: Guid, UpdateUserRequest     | UserResponse         |
| DELETE     | /user/{id}                   | Удалить пользователя           | id: Guid                        | 204 No Content       |
| PUT        | /user/{id}/change-password   | Сменить пароль пользователя    | id: Guid, ChangePasswordRequest | 200 OK               |

| HTTP метод | Путь запроса          | Описание                  | Параметры запроса           | Ответ                |
| ---------- | --------------------- | ------------------------- | --------------------------- | -------------------- |
| GET        | /role?page=&pageSize= | Получить все роли         | page, pageSize              | List\<RoleResponse\> |
| GET        | /role/{id}            | Получить роль по id       | id: Guid                    | RoleResponse         |
| GET        | /role/by-name/{name}  | Получить роль по названию | name: string                | RoleResponse         |
| POST       | /role                 | Создать новую роль        | CreateRoleRequest           | RoleResponse         |
| PUT        | /role/{id}            | Обновить роль             | id: Guid, UpdateRoleRequest | RoleResponse         |
| DELETE     | /role/{id}            | Удалить роль              | id: Guid                    | 204 No Content       |

### **Course Management Service (Сервис управления курсами)**
- Создание и редактирование курсов
- Управление структурой курса (модули, уроки, материалы)
- Загрузка и организация учебного контента
- Католог и поиск курсов
- Версионность контента

| HTTP метод | Путь запроса                             | Описание                  | Параметры запроса                | Ответ                     |
| ---------- | ---------------------------------------- | ------------------------- | -------------------------------- | ------------------------- |
| GET        | /courses?page=&pageSize=                 | Получить все курсы        | page, pageSize                   | List\<CourseResponse\> |
| GET        | /courses/{id}                            | Получить курс по id       | id: Guid                         | CourseResponse         |
| GET        | /courses/by-author/{authorId}            | Получить все курсы автора | authorId: Guid                   | List\<CourseResponse\> |
| GET        | /courses/search?filters=&page=&pageSize= | Поиск курсов              | filters, page, pageSize          | List\<CourseResponse\> |
| POST       | /courses                                 | Создать новый курс        | CreateCourseRequest           | CourseResponse         |
| PUT        | /courses/{id}                            | Обновить курс             | id: Guid, UpdateCourseRequest | CourseResponse         |
| DELETE     | /courses/{id}                            | Удалить курс              | id: Guid                         | 204 No Content            |

| HTTP метод | Путь запроса       | Описание              | Параметры запроса                      | Ответ             |
| ---------- | ------------------ | --------------------- | -------------------------------------- | ----------------- |
| GET        | /modules/{id}      | Получить модуль по id | id: Guid                               | ModuleResponse |
| POST       | /modules?courseId= | Создать модуль        | courseId: Guid, CreateModuleRequest | ModuleResponse |
| PUT        | /modules/{id}      | Обновить модуль       | id: Guid, UpdateModuleRequest       | ModuleResponse |
| DELETE     | /modules/{id}      | Удалить модуль        | id: Guid                               | 204 No Content    |

\* Список модулей хранятся в курсе 

| HTTP Method | Endpoint           | Описание            | Параметры запроса                     | Ответ             |
| ----------- | ------------------ | ------------------- | ------------------------------------- | ----------------- |
| GET         | /lessons/{id}      | Получить урок по id | id: Guid                              | LessonResponse |
| POST        | /lessons?moduleId= | Создать урок        | moduleId: Guid,CreateLessonRequest | LessonResponse |
| PUT         | /lessons/{id}      | Обновить урок       | id: Guid, UpdateLessonRequest      | LessonResponse |
| DELETE      | /lessons/{id}      | Удалить урок        | id: Guid                              | 204 No Content    |

\* Материал урока хранится в jsonb


\* ??? Или хранить все вопросы в jsonb и искать по id. А как тогда структурировать и принимать с фронта
\*\*  ??? Или вообще в документоориентированной
### **Сourse passing service (Сервис прохождения курсов)**
- Процесс записи студентов на курсы
- Отслеживание прогресса прохождения
- Генерация и выдача сертификатов
- Управление дедлайнами и расписанием
- Статистика по успеваемости
- Создание и управление тестами и заданиями
- Автоматическая проверка ответов
- Выставление оценок и обратная связь
- Банк вопросов и антиплагиат
- Аналитика результатов тестирования

| HTTP Method | Endpoint                                       | Описание                                      | Параметры запроса                    | Ответ                         |
| ----------- | ---------------------------------------------- | --------------------------------------------- | ------------------------------------ | ----------------------------- |
| GET         | /enrollments?page=&pageSize=                   | Все записи на курсы (связи пользователь-курс) | page, pageSize                       | List\<EnrollmentResponse\> |
| GET         | /enrollments/{id}                              | Запись по id                                  | id: Guid                             | EnrollmenResponsetDto         |
| GET         | /enrollments/by-user/{userId}?page=&pageSize=  | Записи пользователя                           | userId: Guid, page, pageSize         | List\<EnrollmentResponse\> |
| GET         | /enrollments/course/{courseId}?page=&pageSize= | Записи на курс                                | page, pageSize, courseId: Guid       | List\<EnrollmentResponse\> |
| POST        | /enrollments                                   | Записаться на курс                            | CreateEnrollmentRequest           | EnrollmentResponse         |
| PUT         | /enrollments/{id}                              | Обновить запись                               | id: Guid, UpdateEnrollmenRequesttDto | EnrollmentResponse         |

\* при отписке или завершении курса нужно обнавлять status в enrollments

| HTTP Method | Endpoint                        | Описание                 | Параметры запроса                            | Ответ                    |
| ----------- | ------------------------------- | ------------------------ | -------------------------------------------- | ------------------------ |
| GET         | /progress/{enrollmentId}        | Прогресс по курсу        | enrollmentId: Guid                           | ProgressResponse      |
| PUT         | /progress/{enrollmentId}        | Обновить прогресс        | enrollmentId: Guid, UpdateProgressRequest | ProgressResponse      |
| POST        | /progress/{enrollmentId}/lesson | Отметить урок пройденным | enrollmentId: Guid, {lessonId: Guid}         | ProgressResponse      |
| GET         | /progress/{enrollmentId}/stats  | Статистика прогресса     | enrollmentId: Guid                           | ProgressStatsResponse |

\* нужно обновлять вместе с прохождением курса 

| HTTP Method | Endpoint                                | Описание                        | Параметры запроса             | Ответ                              |
| ----------- | --------------------------------------- | ------------------------------- | ----------------------------- | ---------------------------------- |
| GET         | /certificates?page=&pageSize=           | Все сертификаты                 | page, pageSize                | List\<CertificateResponse\>     |
| GET         | /certificates/enrollment/{enrollmentId} | Сертификат по id записи на курс | id: Guid                      | CertificateResponse             |
| GET         | /certificates/{id}                      | Сертификат по id                | id: Guid                      | CertificateResponse             |
| GET         | /certificates/user/{userId}             | Сертификаты пользователя        | userId: Guid                  | List\<CertificateResponse\>     |
| POST        | /certificates                           | Создать сертификат              | CertificateGenerateRequest | CertificateResponse, File (PDF) |
| GET         | /certificates/{id}/download             | Скачать сертификат              | id: Guid                      | File (PDF)                         |

\* ??? Или объединить эти 3 таблицы(записи, прогресс, сертификаты) в 2 или в 1, или тогда будет много пустых данных до завершения курса

| HTTP Method | Endpoint                                   | Описание                | Параметры запроса                    | Ответ                         |
| ----------- | ------------------------------------------ | ----------------------- | ------------------------------------ | ----------------------------- |
| GET         | /submissions?page=&pageSize=               | Все ответы студентов    | page, pageSize                       | List\<SubmissionResponse\> |
| GET         | /submissions/{id}                          | Ответ по id             | id: Guid                             | SubmissionResponse         |
| GET         | /submissions/user/{userId}?page=&pageSize= | Ответы студента на тест | userId: Guid, page, pageSize         | List\<SubmissionResponse\> |
| POST        | /submissions                               | Отправить ответы        | CreateSubmissionRequest           | SubmissionResponse         |
| PUT         | /submissions/{id}                          | Обновить ответ          | id: Guid, UpdateSubmissionRequest | SubmissionResponse         |

| HTTP Method | Endpoint                             | Описание         | Параметры запроса              | Ответ                    |
| ----------- | ------------------------------------ | ---------------- | ------------------------------ | ------------------------ |
| GET         | /grades?userid=&page=&pageSize=      | Все оценки       | page, pageSize, userId         | List\<GradeResponse\> |
| GET         | /grades/{id}                         | Оценка по id     | id: Guid                       | GradeResponse         |
| GET         | /grades/by-submission/{submissionId} | Оценка по ответу | submissionId: Guid             | GradeResponse         |
| POST        | /grades                              | Выставить оценку | CreateGradeRequest          | GradeResponse         |
| PUT         | /grades/{id}                         | Обновить оценку  | id: Guid,UpdateGradeRequest | GradeResponse         |
| DELETE      | /grades/{id}                         | Удалить оценку   | id: Guid                       | 204 No Content           |

### **Payment Service (Платежный сервис)**
- Обработка платежей
- Управление подписками и платежами
- Выставление счетов и возвраты
- Интеграция с налоговыми системами
- Финансовая отчетность

| HTTP Method | Endpoint                  | Описание             | Параметры запроса              | Ответ                      |
| ----------- | ------------------------- | -------------------- | ------------------------------ | -------------------------- |
| GET         | /payments?page=&pageSize= | История платежей     | page, pageSize, userId, status | List\<PaymentResponse\> |
| GET         | /payments/{id}            | Платеж по id         | id: Guid                       | PaymentResponse         |
| GET         | /payments/user/{userId}   | Платежи пользователя | userId: Guid                   | List\<PaymentResponse\> |
| POST        | /payments/process         | Обработать платеж    | ProcessPaymentRequest       | PaymentResponse         |
| POST        | /payments/{id}/refund     | Возврат средств      | id: Guid, RefundRequest     | PaymentResponse         |
| GET         | /payments/{id}/receipt    | Получить чек         | id: Guid                       | File (PDF)                 |

| HTTP Method | Endpoint                                       | Описание          | Параметры запроса                      | Ответ                           |
| ----------- | ---------------------------------------------- | ----------------- | -------------------------------------- | ------------------------------- |
| GET         | /subscriptions?page=&pageSize=&status=&userid= | Все подписки      | page, pageSize, userId, status         | List\<SubscriptionResponse\> |
| GET         | /subscriptions/{id}                            | Подписка по ID    | id: Guid                               | SubscriptionResponse         |
| POST        | /subscriptions                                 | Создать подписку  | CreateSubscriptionRequest           | SubscriptionResponse         |
| PUT         | /subscriptions/{id}                            | Обновить подписку | id: Guid, UpdateSubscriptionRequest | SubscriptionResponse         |


| HTTP Method | Endpoint                                 | Описание               | Параметры запроса              | Ответ                      |
| ----------- | ---------------------------------------- | ---------------------- | ------------------------------ | -------------------------- |
| GET         | /receipt?page=&pageSize=&status=&userid= | Все чеки               | page, pageSize, userId, status | List\<InvoiceResponse\> |
| GET         | /receipt/{id}                            | Счет по ID             | id: Guid                       | InvoiceResponse         |
| POST        | /receipt/generate                        | Сгенерировать чек      | GenerateInvoiceRequest      | InvoiceсDto                |
| GET         | /receipt/{id}/download                   | Скачать чек            | id: Guid                       | File (PDF)                 |
| POST        | /receipt/{id}/send                       | Отправить чек по email | id: Guid                       | 200 OK                     |

### **Notification Service (Сервис уведомлений)**
- Уведомления на сайте
- Email уведомления
- Планирование и персонализация сообщений
- Управление шаблонами и локализацией

| HTTP Method | Endpoint                                       | Описание                  | Параметры запроса              | Ответ                           |
| ----------- | ---------------------------------------------- | ------------------------- | ------------------------------ | ------------------------------- |
| GET         | /notifications?page=&pageSize=&status=&userid= | Все уведомления           | page, pageSize, userId, status | List\<NotificationResponse\> |
| GET         | /notifications/{id}                            | Уведомление по ID         | id: Guid                       | NotificationResponse         |
| GET         | /notifications/user/{userId}                   | Уведомления пользователя  | userId: Guid                   | List\<NotificationResponse\> |
| POST        | /notifications/send                            | Отправить уведомление     | SendNotificationRequest     | NotificationResponse         |
| POST        | /notifications/send-many                       | Массовая рассылка         | SendManyNotificationRequest | 200 OK                          |
| PUT         | /notifications/{id}/mark-read                  | Отметить прочитанным      | id: Guid                       | NotificationResponse         |
| PUT         | /notifications/user/{userId}/mark-all-read     | Отметить все прочитанными | userId: Guid                   | 200 OK                          |
| DELETE      | /notifications/{id}                            | Удалить уведомление       | id: Guid                       | 204 No Content                  |

| HTTP Method | Endpoint                                      | Описание        | Параметры запроса                  | Ответ                       |
| ----------- | --------------------------------------------- | --------------- | ---------------------------------- | --------------------------- |
| GET         | /notification-templates?page=&pageSize=&type= | Все шаблоны     | page, pageSize, type               | List\<TemplateResponse\> |
| GET         | /notification-templates/{id}                  | Шаблон по ID    | id: Guid                           | TemplateResponse         |
| POST        | /notification-templates                       | Создать шаблон  | CreateTemplateDto                  | TemplateResponse         |
| PUT         | /notification-templates/{id}                  | Обновить шаблон | id: Guid, UpdateTemplateRequest | TemplateResponse         |
| DELETE      | /notification-templates/{id}                  | Удалить шаблон  | id: Guid                           | 204 No Content              |

## **Сущности User Management Service**

<img width="739" height="636" alt="{445E30A8-64B4-4065-9891-E68E6162F1C4}" src="https://github.com/user-attachments/assets/cdabe090-ee81-487a-b087-c12cad6d25fc" />
