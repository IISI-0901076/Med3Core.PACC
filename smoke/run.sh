#!/bin/sh
set -e

echo "=== PACC WebAPI Smoke Test ==="

# 1) 等待 API healthy
echo "[INFO] Waiting for API..."
for i in $(seq 1 30); do
    if curl -fsS "${API_BASE}/health" >/dev/null 2>&1; then
        echo "[INFO] API is healthy"
        break
    fi
    sleep 2
done

# DevAuthHandler 會自動通過認證，不需 token
TOKEN="dev-bypass"

# Helper function
assert_ok() {
    ENDPOINT=$1
    PAYLOAD=$2
    BODY=$(curl -fsS -X POST "${API_BASE}/pacc1000/PACC1001N01/${ENDPOINT}" \
        -H "Authorization: Bearer ${TOKEN}" \
        -H "Content-Type: application/json" \
        -d "${PAYLOAD}")
    RTN_CODE=$(echo "$BODY" | sed -n 's/.*"rtnCode":"\([^"]*\)".*/\1/p')
    if [ "$RTN_CODE" != "0" ]; then
        echo "[FAIL] ${ENDPOINT}: ${BODY}"
        exit 1
    fi
    echo "[OK] ${ENDPOINT}"
}

# 2) 依相依順序打 10 個端點
TS=$(date +%s)
CASE="SMOKE${TS}"
RECV="REC${TS}"

assert_ok "CreateRecvLog" "{\"recvSeqNo\":\"${RECV}\",\"busCode\":\"IPL\",\"hospId\":\"0401\",\"fileType\":\"XML\",\"recvMode\":\"FTP\",\"readPos\":\"1\"}"

assert_ok "CreateCase" "{\"caseSeqNo\":\"${CASE}\",\"busCode\":\"IPL\",\"hospId\":\"0401\",\"caseStatus\":\"0\",\"recvSeqNo\":\"${RECV}\",\"readPos\":\"1\"}"

assert_ok "CreateAttFile" "{\"caseSeqNo\":\"${CASE}\",\"itemNo\":1,\"fileName\":\"test.xml\",\"fileType\":\"XML\",\"fileObjectKey\":\"IPL/0401/${CASE}/test.xml\"}"

assert_ok "QueryStatus" "{\"caseSeqNo\":\"${CASE}\",\"pageNo\":1,\"pageSize\":10}"

assert_ok "UpdateCaseStatus" "{\"caseSeqNo\":\"${CASE}\",\"pgmProcStatus\":\"9\"}"

assert_ok "UpdateCaseFiles" "{\"caseSeqNo\":\"${CASE}\",\"fileObjectKey\":\"IPL/0401/${CASE}/test.xml\"}"

assert_ok "QueryCaseFiles" "{\"caseSeqNo\":\"${CASE}\"}"

assert_ok "UpdateFileLocation" "{\"caseSeqNo\":\"${CASE}\",\"fileObjectKey\":\"IPL/0401/${CASE}/output.xml\"}"

assert_ok "UpdateRecvLog" "{\"recvSeqNo\":\"${RECV}\",\"procResult\":\"1\"}"

assert_ok "LogError" "{\"caseSeqNo\":\"${CASE}\",\"errCode\":\"E400\",\"procErrMsg\":\"smoke test error\",\"errorPhase\":\"PGM\"}"

echo ""
echo "=== ALL 10 ENDPOINTS PASSED ==="
