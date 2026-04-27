using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeGame2.Shared.Saves;

namespace AwesomeGame2.Shared.Validation
{
    public static class GameSaveValidator
    {
        public static IReadOnlyList<string> Validate(GameSave save)
        {
            var errors = new List<string>();

            if (save == null)
            {
                errors.Add("GameSave is required.");
                return errors;
            }

            if (save.SaveVersion <= 0)
            {
                errors.Add("SaveVersion must be greater than zero.");
            }

            if (save.Player == null)
            {
                errors.Add("Player is required.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(save.Player.Name))
                {
                    errors.Add("Player.Name is required.");
                }

                if (save.Player.Level < 1)
                {
                    errors.Add("Player.Level must be at least 1.");
                }

                if (save.Player.Health < 0 || save.Player.MaxHealth < 1 || save.Player.Health > save.Player.MaxHealth)
                {
                    errors.Add("Player health values are invalid.");
                }

                if (save.Player.BaseAttack < 1)
                {
                    errors.Add("Player.BaseAttack must be at least 1.");
                }
            }

            if (string.IsNullOrWhiteSpace(save.CurrentLocation))
            {
                errors.Add("CurrentLocation is required.");
            }

            if (save.DayNumber < 1)
            {
                errors.Add("DayNumber must be at least 1.");
            }

            if (save.RemainingDailyActions < 0)
            {
                errors.Add("RemainingDailyActions cannot be negative.");
            }

            if (save.Inventory == null)
            {
                errors.Add("Inventory is required.");
            }
            else if (save.Inventory.Any(i => i.Quantity < 0))
            {
                errors.Add("Inventory quantities cannot be negative.");
            }

            return errors;
        }

        public static bool IsValid(GameSave save)
        {
            return Validate(save).Count == 0;
        }
    }
}
