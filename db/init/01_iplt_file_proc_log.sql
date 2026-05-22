-- 01_iplt_file_proc_log.sql: 案件處理記錄表
CREATE TABLE IF NOT EXISTS iplt_file_proc_log (
    case_seq_no       VARCHAR(20)   PRIMARY KEY,
    bus_code          VARCHAR(10)   NOT NULL,
    hosp_id           VARCHAR(10),
    branch_code       VARCHAR(10),
    case_status       VARCHAR(2)    NOT NULL DEFAULT '0',
    pgm_proc_status   VARCHAR(2)    NOT NULL DEFAULT '0',
    file_send_status  VARCHAR(2)    NOT NULL DEFAULT '0',
    recv_seq_no       VARCHAR(20),
    read_pos          VARCHAR(2),
    hl7               VARCHAR(200),
    upload_format     VARCHAR(10),
    add_time          TIMESTAMP     DEFAULT CURRENT_TIMESTAMP,
    pgm_time_s        TIMESTAMP,
    pgm_time_e        TIMESTAMP,
    file_time_s       TIMESTAMP,
    file_time_e       TIMESTAMP,
    proc_pos          VARCHAR(500),
    err_code          VARCHAR(10),
    proc_err_msg      VARCHAR(500)
);

CREATE INDEX IF NOT EXISTS idx_file_proc_log_add_time
    ON iplt_file_proc_log (add_time DESC);

CREATE INDEX IF NOT EXISTS idx_file_proc_log_status
    ON iplt_file_proc_log (bus_code, case_status, pgm_proc_status, file_send_status);
