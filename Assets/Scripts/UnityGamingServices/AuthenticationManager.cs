using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject
{
    public static class AuthenticationManager
    {
        #region Fields

        //public static event Action<string> OccuredError;

        //public static bool canSigninAnonymously = false;

        //public static bool canDelete = false;

        #endregion

        #region Methods

        /// <summary>
        /// Unityサービスにログイン中か確認
        /// </summary>
        /// <returns></returns>
        //public static bool IsSignedInUnityservices()
        //{
        //    bool status = AuthenticationService.Instance != null ? AuthenticationService.Instance.IsSignedIn : false;

        //    return status;
        //}

        /// <summary>
        /// プレイヤーのIDを取得
        /// </summary>
        /// <returns></returns>
        //public static string GetPlayerID()
        //{
        //    return AuthenticationService.Instance.PlayerId;
        //}

        /// <summary>
        /// プレイヤー名を取得
        /// </summary>
        /// <returns></returns>
        //public static string GetPlayerName()
        //{
        //    return AuthenticationService.Instance.PlayerName;
        //}

        /// <summary>
        /// Unityサービスの初期化
        /// </summary>
        /// <returns></returns>
        //public static async Task InitializeUnityServicesAsync()
        //{
        //    try
        //    {
        //        await UnityServices.InitializeAsync();
        //    }
        //    catch (AuthenticationException ex)
        //    {
        //        //イベントの呼び出し（エラー通知UIの表示）
        //        OccuredError?.Invoke(ex.Message);
        //    }
        //    catch (RequestFailedException ex)
        //    {
        //        //イベントの呼び出し（エラー通知UIの表示）
        //        OccuredError?.Invoke(ex.Message);
        //    }
        //}

        /// <summary>
        /// 匿名ログイン
        /// </summary>
        /// <returns></returns>
        //public static async Task SignInAnonymouslyAsync()
        //{
        //    try
        //    {
        //        //匿名ログイン
        //        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        //        canSigninAnonymously = true;
        //    }
        //    catch (AuthenticationException ex)
        //    {
        //        canSigninAnonymously = false;

        //        //イベントの呼び出し（エラー通知UIの表示）
        //        OccuredError?.Invoke(ex.Message);
        //    }
        //    catch (RequestFailedException ex)
        //    {
        //        canSigninAnonymously = false;

        //        //イベントの呼び出し（エラー通知UIの表示）
        //        OccuredError?.Invoke(ex.Message);
        //    }
        //}

        /// <summary>
        /// ユーザーのサインアウト
        /// </summary>
        /// <returns></returns>
        //public static void SignOut()
        //{
        //    AuthenticationService.Instance.SignOut();
        //}

        /// <summary>
        /// ユーザーのセッショントークンの初期化
        /// </summary>
        /// <returns></returns>
        //public static void ClearSessionToken()
        //{
        //    AuthenticationService.Instance.ClearSessionToken();
        //}

        /// <summary>
        /// アカウントデータの削除
        /// </summary>
        /// <returns></returns>
        //public static async Task DeleteAccountAsync()
        //{
        //    try
        //    {
        //        await AuthenticationService.Instance.DeleteAccountAsync();

        //        canDelete = true;
        //    }
        //    catch (AuthenticationException ex)
        //    {
        //        canDelete = false;
        //    }
        //    catch (RequestFailedException ex)
        //    {
        //        canDelete = false;
        //    }
        //}

        #endregion
    }
}
