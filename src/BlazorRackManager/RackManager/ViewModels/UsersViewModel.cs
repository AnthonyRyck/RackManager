using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using RackManager.Data;
using Serilog;
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
		public bool IsLoaded { get; set; }

		public List<UserView> AllUsers { get; set; }

		private string IdUserToChangePassword { get; set; }

		#endregion

		#region Constructeur

		public UsersViewModel(ApplicationDbContext appContext, UserManager<IdentityUser> userManager, NavigationManager navigation)
		{
			ShowResetMdp = false;
			IsLoaded = false;

			AppContext = appContext;
			UserManager = userManager;
			Navigation = navigation;
		}

		#endregion

		public async Task LoadAllUsers()
		{
			IsLoaded = false;
			AllUsers = await GetAllUser();
			IsLoaded = true;
		}

		/// <summary>
		/// Au changement de rôle.
		/// </summary>
		/// <param name="e"></param>
		/// <param name="idUser"></param>
		public async Task OnChangeRole(ChangeEventArgs e, string idUser)
		{
			try
			{
				var selectedValue = e.Value.ToString();
				UserView currentUser = AllUsers.Where(x => x.User.Id == idUser).FirstOrDefault();

				if (string.IsNullOrEmpty(selectedValue))
					return;

				Log.Information("USER - Changement ROLE pour {username} - ancien {role} - nouveau {selectedValue} ", currentUser.User.UserName, currentUser.Role, selectedValue);

				// Utilisateur qui a déjà un rôle.
				if (currentUser.Role != null)
					await UserManager.RemoveFromRoleAsync(currentUser.User, currentUser.Role);

				if (selectedValue != "SansRole")
				{
					await UserManager.AddToRoleAsync(currentUser.User, selectedValue);
				}

				AllUsers = await GetAllUser();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "UserViewModel - OnChangeRole");
			}
		}

		public void DeleteUser(string idUser)
		{
			try
			{
				if (AppContext.Users.Any(x => x.Id == idUser))
				{
					var user = AppContext.Users.FirstOrDefault(x => x.Id == idUser);
					Log.Information("USER - Suppression de l'utilisateur : {username}", user.UserName);

					AppContext.Users.Remove(user);
					AppContext.SaveChanges();

					AllUsers.RemoveAll(x => x.User.Id == idUser);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "UserViewModel - DeleteUser");
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

				Log.Information("USER - Changement de mot de passe pour : {username}", userSelected.UserName);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "UserViewModel - SetNewPassword");
			}

			IdUserToChangePassword = string.Empty;
		}

		#region Private Methods

		/// <summary>
		/// Retourne la liste des Managers et des membres.
		/// </summary>
		/// <returns></returns>
		private async Task<List<UserView>> GetAllUser()
		{
			List<UserView> usersList = new List<UserView>();

			try
			{
				usersList = await Task<List<UserView>>.Factory.StartNew(() => 
				{
					List<UserView> retourUsers = new List<UserView>();
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

						retourUsers.Add(userView);
					}

					return retourUsers;
				});
				
			}
			catch (Exception ex)
			{
				Log.Error(ex, "UserViewModel - GetAllUser (private)");
			}

			return usersList;
		}

		#endregion
	}
}
