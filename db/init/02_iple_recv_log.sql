-- 02_iple_recv_log.sql: 收件記錄表
CREATE TABLE IF NOT EXISTS iple_recv_log (
    recv_seq_no       VARCHAR(20)   PRIMARY KEY,
    bus_code          VARCHAR(10)   NOT NULL,
    hosp_id           VARCHAR(10),
    branch_code       VARCHAR(10),
    file_type         VARCHAR(10),
    recv_mode         VARCHAR(5),
    orig_file_name    VARCHAR(200),
    file_size         NUMERIC,
    file_object_key   VARCHAR(500),
    sam_id            VARCHAR(20),
    job_id            VARCHAR(20),
    pc_code           VARCHAR(20),
    read_pos          VARCHAR(2),
    add_time          TIMESTAMP     DEFAULT CURRENT_TIMESTAMP,
    proc_result       VARCHAR(2),
    proc_time         TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_recv_log_bus_hosp
    ON iple_recv_log (bus_code, hosp_id);
