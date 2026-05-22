-- 04_iple_recv_att_file.sql: 遠端附件表 (nhi_idc schema)
CREATE TABLE IF NOT EXISTS nhi_idc.iple_recv_att_file (
    case_seq_no       VARCHAR(20)   NOT NULL,
    item_no           NUMERIC       NOT NULL,
    file_name         VARCHAR(200),
    file_type         VARCHAR(10),
    file_object_key   VARCHAR(500),
    file_memo         VARCHAR(500),
    hl7               VARCHAR(200),
    orig_file_name    VARCHAR(200),
    file_attach       VARCHAR(10),
    document_type     VARCHAR(10),
    PRIMARY KEY (case_seq_no, item_no)
);
