using CommunityToolkit.Mvvm.ComponentModel;
using Genshin_Calculator.Models;
using System;

namespace Genshin_Calculator.ViewModels;

public partial class SkillViewModel : ObservableObject
{
    [ObservableProperty]
    private int currentLevel;

    [ObservableProperty]
    private int desiredLevel;

    public SkillViewModel(Skill model)
    {
        this.Model = model;

        this.CurrentLevel = model.CurrentLevel;
        this.DesiredLevel = model.DesiredLevel;
    }

    public Skill Model { get; }

    partial void OnCurrentLevelChanged(int value)
    {
        if (value > desiredLevel)
        {
            Model.DesiredLevel = Model.CurrentLevel;
        }

        Model.CurrentLevel = Math.Clamp(value, 1, 10);
    }

    partial void OnDesiredLevelChanged(int value)
    {
        if (Model.CurrentLevel > value)
        {
            Model.CurrentLevel = value;
        }

        Model.DesiredLevel = Math.Clamp(value, 1, 10);
    }
}