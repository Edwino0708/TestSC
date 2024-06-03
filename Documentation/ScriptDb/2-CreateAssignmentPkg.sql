CREATE OR REPLACE PACKAGE AssignmentPkg IS
    PROCEDURE CreateAssignment(
        p_Title IN VARCHAR2,
        p_Description IN VARCHAR2,
        p_DueDate IN DATE,
        p_Status IN VARCHAR2,        
        p_ID OUT NUMBER
    );
    
    PROCEDURE ReadAllAssignments(
        p_Cursor OUT SYS_REFCURSOR
    );
    
    PROCEDURE ReadAssignment(
        p_Id IN NUMBER,
        p_Title OUT VARCHAR2,
        p_Description OUT VARCHAR2,
        p_CreationDate OUT DATE,
        p_DueDate OUT DATE,
        p_Status OUT VARCHAR2
    );
    
    PROCEDURE UpdateAssignment(
        p_Id IN NUMBER,
        p_Title IN VARCHAR2,
        p_Description IN VARCHAR2,
        p_DueDate IN DATE,
        p_Status IN VARCHAR2
    );
    
    PROCEDURE DeleteAssignment(
        p_Id IN NUMBER
    );
END AssignmentPkg;
/

CREATE OR REPLACE PACKAGE BODY AssignmentPkg IS
    PROCEDURE CreateAssignment(
        p_Title IN VARCHAR2,
        p_Description IN VARCHAR2,
        p_DueDate IN DATE,
        p_Status IN VARCHAR2,
        p_ID OUT NUMBER
    ) IS
    BEGIN
        INSERT INTO Assignment (Title, Description, DueDate, Status)
        VALUES (p_Title, p_Description, p_DueDate, p_Status)
        RETURNING ID INTO p_ID;
    END CreateAssignment;
    
    PROCEDURE ReadAllAssignments(
        p_Cursor OUT SYS_REFCURSOR
    ) IS
    BEGIN
        OPEN p_Cursor FOR
        SELECT Id, Title, Description, CreationDate, DueDate, Status
        FROM Assignment;
    END ReadAllAssignments;
        
    PROCEDURE ReadAssignment(
        p_Id IN NUMBER,
        p_Title OUT VARCHAR2,
        p_Description OUT VARCHAR2,
        p_CreationDate OUT DATE,
        p_DueDate OUT DATE,
        p_Status OUT VARCHAR2
    ) IS
    BEGIN
        SELECT Title, Description, CreationDate, DueDate, Status
        INTO p_Title, p_Description, p_CreationDate, p_DueDate, p_Status
        FROM Assignment
        WHERE Id = p_Id;
    END ReadAssignment;
    
    PROCEDURE UpdateAssignment(
        p_Id IN NUMBER,
        p_Title IN VARCHAR2,
        p_Description IN VARCHAR2,
        p_DueDate IN DATE,
        p_Status IN VARCHAR2
    ) IS
    BEGIN
        UPDATE Assignment
        SET Title = p_Title,
            Description = p_Description,
            DueDate = p_DueDate,
            Status = p_Status
        WHERE Id = p_Id;
    END UpdateAssignment;
    
    PROCEDURE DeleteAssignment(
        p_Id IN NUMBER
    ) IS
    BEGIN
        DELETE FROM Assignment
        WHERE Id = p_Id;
    END DeleteAssignment;
END AssignmentPkg;
/
