-- Ejemplo de llamada a los procedimientos del paquete AssignmentPkg

-- Llamar al procedimiento CreateAssignment
BEGIN
    AssignmentPkg.CreateAssignment(
        p_Title => 'Tarea 1',
        p_Description => 'Descripción de la tarea 1',
        p_DueDate => TO_DATE('2024-06-01', 'YYYY-MM-DD'),
        p_Status => 'Pendiente'
    );
END;
/

-- Llamar al procedimiento ReadAssignment
SET SERVEROUTPUT ON;
DECLARE
    v_Title VARCHAR2(255);
    v_Description VARCHAR2(1000);
    v_CreationDate DATE;
    v_DueDate DATE;
    v_Status VARCHAR2(50);
BEGIN
    AssignmentPkg.ReadAssignment(
        p_Id => 2,
        p_Title => v_Title,
        p_Description => v_Description,
        p_CreationDate => v_CreationDate,
        p_DueDate => v_DueDate,
        p_Status => v_Status
    );
    DBMS_OUTPUT.PUT_LINE('Title: ' || v_Title);
    DBMS_OUTPUT.PUT_LINE('Description: ' || v_Description);
    DBMS_OUTPUT.PUT_LINE('Creation Date: ' || TO_CHAR(v_CreationDate, 'DD-MON-YYYY HH24:MI:SS'));
    DBMS_OUTPUT.PUT_LINE('Due Date: ' || TO_CHAR(v_DueDate, 'DD-MON-YYYY HH24:MI:SS'));
    DBMS_OUTPUT.PUT_LINE('Status: ' || v_Status);
END;
/


-- Llamar al procedimiento UpdateAssignment
BEGIN
    AssignmentPkg.UpdateAssignment(
        Id => 1,
        p_Title => 'Tarea 1 Actualizada',
        p_Description => 'Descripción actualizada de la tarea 1',
        p_DueDate => TO_DATE('2024-06-02', 'YYYY-MM-DD'),
        p_Status => 'En progreso'
    );
END;
/

-- Llamar al procedimiento DeleteAssignment
BEGIN
    AssignmentPkg.DeleteAssignment(
        Id => 1
    );
END;
/
