﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using RackManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RackManager.ViewModels
{
	public class UsersViewModel : IUsersViewModel
	{
		#region Properties

		private ApplicationDbContext AppContext { get; set; }

		public NavigationManager Navigation { get; set; }

		public UserManager<IdentityUser> UserManager { get; set; }

		public bool ShowResetMdp { get; set; }

		public List<UserView> AllUsers { get; set; }

		private string IdUserToChangePassword { get; set; }

		#endregion

		#region Constructeur

		public UsersViewModel(ApplicationDbContext appContext, UserManager<IdentityUser> userManager, NavigationManager navigation)
		{
			ShowResetMdp = false;

			AppContext = appContext;
			UserManager = userManager;
			Navigation = navigation;
		}

		#endregion

		public void LoadAllUsers()
		{
			AllUsers = GetAllUser().ToList();
		}

		/// <summary>
		/// Au changement de rôle.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="idUser"></param>
		public void OnChangeRole(ChangeEventArgs e, string idUser)
		{
			try
			{
				var selectedValue = e.Value.ToString();
				UserView currentUser = AllUsers.Where(x => x.User.Id == idUser).FirstOrDefault();

				if (string.IsNullOrEmpty(selectedValue))
					return;

				if (selectedValue == "SansRole")
				{
					UserManager.RemoveFromRoleAsync(currentUser.User, currentUser.Role);
				}
				else
				{
					UserManager.RemoveFromRoleAsync(currentUser.User, currentUser.Role);
					UserManager.AddToRoleAsync(currentUser.User, selectedValue);
				}

				AllUsers = GetAllUser().ToList();
			}
			catch (Exception ex)
			{
				
			}
		}

		public void DeleteUser(string idUser)
		{
			if (AppContext.Users.Any(x => x.Id == idUser))
			{
				var user = AppContext.Users.FirstOrDefault(x => x.Id == idUser);

				AppContext.Users.Remove(user);
				AppContext.SaveChanges();

				AllUsers.RemoveAll(x => x.User.Id == idUser);
			}
		}

		public void OpenChangeMdp(string idUser)
		{
			ShowResetMdp = true;
			IdUserToChangePassword = idUser;
		}

		public void CancelChangeMdp()
		{
			IdUserToChangePassword = string.Empty;
			ShowResetMdp = false;
		}


		public async Task SetNewPassword(string newPassword)
		{
			ShowResetMdp = false;

			try
			{
				IdentityUser userSelected = AppContext.Users.Where(x => x.Id == IdUserToChangePassword).FirstOrDefault();
				await UserManager.RemovePasswordAsync(userSelected);
				await UserManager.AddPasswordAsync(userSelected, newPassword);
			}
			catch (Exception)
			{
				
			}

			IdUserToChangePassword = string.Empty;
		}

		#region Private Methods

		/// <summary>
		/// Retourne la liste des Managers et des membres.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<UserView> GetAllUser()
		{
			List<UserView> usersList = new List<UserView>();

			try
			{
				// Récupération des utilisateurs.
				IEnumerable<IdentityUser> usersTemp = AppContext.Users.ToList();

				foreach (var user in usersTemp)
				{
					string roleId = AppContext.UserRoles.Where(x => x.UserId == user.Id)
											.Select(x => x.RoleId)
											.FirstOrDefault();

					string role = AppContext.Roles.Where(x => x.Id == roleId)
													.Select(x => x.NormalizedName)
													.FirstOrDefault();

					UserView userView = new UserView();
					userView.User = user;
					userView.Role = role;

					usersList.Add(userView);
				}
			}
			catch (Exception exception)
			{
				
			}

			return usersList;
		}

		#endregion
	}
}
