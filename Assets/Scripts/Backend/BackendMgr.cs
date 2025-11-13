using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using TMPro;

public class BackendMgr : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField txtfId, txtfPw;

    [SerializeField]
    private TextMeshProUGUI errorMessageText;

    // ID 조건
    private const int MIN_ID_LENGTH = 6;
    private const int MAX_ID_LENGTH = 12;

    void Start()
    {
        var bro = Backend.Initialize();

        if(bro.IsSuccess())
            Debug.Log("Initialize Succ");
        else
            Debug.Log("Initialize Fail");
    }

    /// <summary>
    /// 아이디 입력값 검증
    /// </summary>
    private bool ValidateIdInput(string id)
    {
        // 빈 텍스트 확인
        if (string.IsNullOrWhiteSpace(id))
        {
            ShowError("아이디를 입력해주세요.");
            return false;
        }

        // 최소 길이 확인 (6자 이상)
        if (id.Length < MIN_ID_LENGTH)
        {
            ShowError($"아이디는 최소 {MIN_ID_LENGTH}자 이상이어야 합니다.");
            return false;
        }

        // 최대 길이 확인 (12자 이하)
        if (id.Length > MAX_ID_LENGTH)
        {
            ShowError($"아이디는 {MAX_ID_LENGTH}자 이하여야 합니다.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 비밀번호 입력값 검증
    /// </summary>
    private bool ValidatePasswordInput(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            ShowError("비밀번호를 입력해주세요.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 에러 메시지 표시
    /// </summary>
    private void ShowError(string errorMessage)
    {
        Debug.LogWarning($"[Error] {errorMessage}");

        if (errorMessageText != null)
        {
            errorMessageText.text = errorMessage;
            errorMessageText.color = Color.red;
        }
    }

    /// <summary>
    /// 성공 메시지 표시
    /// </summary>
    private void ShowSuccess(string successMessage)
    {
        Debug.Log($"[Success] {successMessage}");

        if (errorMessageText != null)
        {
            errorMessageText.text = successMessage;
            errorMessageText.color = Color.green;
        }
    }

    /// <summary>
    /// 로그인 버튼 클릭 이벤트
    /// </summary>
    public void OnbtnLogin()
    {
        string id = txtfId.text;
        string password = txtfPw.text;

        // 입력값 검증
        if (!ValidateIdInput(id) || !ValidatePasswordInput(password))
            return;

        Debug.Log($"로그인 시도: {id}");
        StartCoroutine(LoginCoroutine(id, password));
    }

    /// <summary>
    /// 회원가입 버튼 클릭 이벤트
    /// </summary>
    public void OnbtnSignUp()
    {
        string id = txtfId.text;
        string password = txtfPw.text;

        // 입력값 검증
        if (!ValidateIdInput(id) || !ValidatePasswordInput(password))
            return;

        Debug.Log($"회원가입 시도: {id}");
        StartCoroutine(SignUpCoroutine(id, password));
    }

    /// <summary>
    /// 로그인 코루틴
    /// </summary>
    private IEnumerator LoginCoroutine(string id, string password)
    {
        var bro = Backend.BMember.CustomLogin(id, password);

        if (bro.IsSuccess())
        {
            ShowSuccess("로그인에 성공했습니다!");
            Debug.Log("로그인 성공");
            txtfId.text = "";
            txtfPw.text = "";
            // TODO: 로그인 후 처리 (씬 이동 등)
        }
        else
        {
            HandleAuthError(bro);
        }

        yield return null;
    }

    /// <summary>
    /// 회원가입 코루틴
    /// </summary>
    private IEnumerator SignUpCoroutine(string id, string password)
    {
        var bro = Backend.BMember.CustomSignUp(id, password);

        if (bro.IsSuccess())
        {
            ShowSuccess("회원가입에 성공했습니다! 로그인해주세요.");
            Debug.Log("회원가입 성공");
            // 회원가입 후 입력필드 초기화
            txtfId.text = "";
            txtfPw.text = "";
        }
        else
        {
            HandleAuthError(bro);
        }

        yield return null;
    }

    /// <summary>
    /// 인증 관련 에러 처리
    /// </summary>
    private void HandleAuthError(BackendReturnObject bro)
    {
        string errorCode = bro.GetErrorCode();
        string errorMessage = bro.GetErrorMessage();

        Debug.LogError($"에러 코드: {errorCode}, 메시지: {errorMessage}");

        // 에러 코드별 처리
        switch (errorCode)
        {
            case "NOT_EXIST_ACCOUNT":
                ShowError("존재하지 않는 아이디입니다.");
                break;

            case "WRONG_PASSWORD":
                ShowError("비밀번호가 틀렸습니다.");
                break;

            case "EXPIRED_TOKEN":
                ShowError("액세스 토큰이 만료되었습니다. 다시 로그인해주세요.");
                break;

            case "ALREADY_EXIST_ACCOUNT":
                ShowError("이미 존재하는 아이디입니다.");
                break;

            case "INVALID_ID_FORMAT":
                ShowError("아이디 형식이 올바르지 않습니다.");
                break;

            case "INVALID_PASSWORD_FORMAT":
                ShowError("비밀번호 형식이 올바르지 않습니다.");
                break;

            default:
                ShowError($"오류가 발생했습니다. ({errorCode})");
                break;
        }
    }

    /// <summary>
    /// 로그아웃
    /// </summary>
    public void OnbtnLogout()
    {
        var bro = Backend.BMember.Logout();

        if (bro.IsSuccess())
        {
            ShowSuccess("로그아웃되었습니다.");
            Debug.Log("로그아웃 성공");
        }
        else
        {
            ShowError("로그아웃에 실패했습니다.");
        }
    }
}