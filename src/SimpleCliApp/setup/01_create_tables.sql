-- Check if the USERS table already exists before creating
BEGIN
   EXECUTE IMMEDIATE 'DROP TABLE USERS PURGE';
EXCEPTION
   WHEN OTHERS THEN
      IF SQLCODE != -942 THEN
         RAISE;
      END IF;
END;
/

-- Create users table
CREATE TABLE USERS (
  ID NUMBER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  NAME VARCHAR2(100) NOT NULL,
  EMAIL VARCHAR2(255) NOT NULL,
  CREATED_DATE DATE DEFAULT SYSDATE
);

-- Grant privileges to the app user
GRANT SELECT, INSERT, UPDATE, DELETE ON USERS TO admin;

-- Insert a test record
INSERT INTO USERS (NAME, EMAIL, CREATED_DATE) 
VALUES ('Test User', 'test@example.com', SYSDATE);

COMMIT;

-- Show the table was created successfully
SELECT * FROM USERS;

EXIT;
