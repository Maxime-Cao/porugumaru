// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let onClickDeleteImage = () => {
    if(confirm("Are you sure you want to delete this picture?")) {
        document.querySelector("#delete-profile-picture").value = true;
        document.querySelector("#profile-picture").style.display = "none";
        document.querySelector("#delete-profile-picture-button").style.display = "none";
        document.querySelector("#profile-picture-title").style.display = "none";
    }
}

let deleteProfilePictureButton = document.querySelector("#delete-profile-picture-button");

if(deleteProfilePictureButton != null) {
    deleteProfilePictureButton.addEventListener("click", () => {
       onClickDeleteImage(); 
    });
}

let onClickDeleteLesson = (event) => {
    if(!confirm("Are you sure you want to delete this lesson?")) {
        event.preventDefault();
    }
}

let deleteLessonLinks = document.querySelectorAll(".delete-lesson");

if(deleteLessonLinks != null) {
    deleteLessonLinks.forEach(link => {
        link.addEventListener("click",(event) => {
            onClickDeleteLesson(event);
        })
    });
}

let onClickShowSolution = (event) => {
    if(!confirm("Are you sure you want to see the solution?")) {
        event.preventDefault();
    }
}

let showSolutionLink = document.querySelector("#show-solution");

if(showSolutionLink != null) {
    showSolutionLink.addEventListener("click",(event) => {
       onClickShowSolution(event); 
    });
}

let onClickDeleteExercise = (event) => {
    if(!confirm("Are you sure you want to delete this exercise?")) {
        event.preventDefault();
    }
}

let deleteExerciseLinks = document.querySelectorAll(".delete-exercise");

if(deleteExerciseLinks != null) {
    deleteExerciseLinks.forEach(link => {
        link.addEventListener("click",(event) => {
            onClickDeleteExercise(event);
        })
    });
}

let onClickResetLesson = (event) => {
    if(!confirm("Are you sure you want to reset this lesson?")) {
        event.preventDefault();
    }
}

let resetLessonLinks = document.querySelectorAll(".reset-lesson");

if(resetLessonLinks != null)
{
    resetLessonLinks.forEach(link => {
        link.addEventListener("click",(event) => {
            onClickResetLesson(event);
        });
    });
}

let onClickResetExercise = (event) => {
    if(!confirm("Are you sure you want to reset this exercise?")) {
        event.preventDefault();
    }
}

let resetExerciseLinks = document.querySelectorAll(".reset-exercise");

if(resetExerciseLinks != null)
{
    resetExerciseLinks.forEach(link => {
        link.addEventListener("click",(event) => {
            onClickResetExercise(event);
        });
    });
}