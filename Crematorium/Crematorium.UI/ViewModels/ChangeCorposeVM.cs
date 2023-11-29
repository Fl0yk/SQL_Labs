﻿using CommunityToolkit.Mvvm.ComponentModel;
using Crematorium.Application.Abstractions;
using Crematorium.Application.Validators;
using Crematorium.Domain.Entities;
using FluentValidation;
using System;
using System.ComponentModel.DataAnnotations;

namespace Crematorium.UI.ViewModels
{
    public partial class ChangeCorposeVM : ObservableValidator
    {
        private IHelpersService<Corpose> _corposeService;
        private bool _isNewCorpose;
        public ChangeCorposeVM(IHelpersService<Corpose> corposeService)
        {
            _corposeService = corposeService;
        }

        [ObservableProperty]
        [Required]
        private Corpose corpose;

        public void SetCorpose(int corposeId)
        {
            Corpose = _corposeService.GetByIdAsync(corposeId).Result;

            if (Corpose is null)
            {
                Corpose = new Corpose();
                _isNewCorpose = true;
            }
            else
            {
                _isNewCorpose = false;
            }
            this.Name = Corpose.Name;
            this.Surname = Corpose.SurName;
            this.NumPassport = Corpose.NumPassport;
        }

        public void SetCorpose(ref Corpose selCorpose)
        {
            if (selCorpose is null)
            {
                Corpose = new Corpose();
                selCorpose = Corpose;
                _isNewCorpose = true;
            }
            else
            {
                this.Corpose = selCorpose;
                _isNewCorpose = false;
            }
            this.Name = Corpose.Name;
            this.Surname = Corpose.SurName;
            this.NumPassport = Corpose.NumPassport;
        }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string surname;

        [ObservableProperty]
        private string numPassport;


        public void AddCorpose()
        {
            if (Corpose is null)
                throw new ArgumentNullException("corpose not initialized");

            Corpose.Name = this.Name;
            Corpose.SurName = this.Surname;
            Corpose.NumPassport = this.NumPassport;

            CorposeValidator validator = new();
            validator.ValidateAndThrow(Corpose);

            if (_isNewCorpose)
            {
                _corposeService.AddAsync(Corpose);
            }
            else
            {
                _corposeService.UpdateAsync(Corpose);
            }
        }
    }
}