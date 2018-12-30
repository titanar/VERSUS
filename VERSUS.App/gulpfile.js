/// <binding BeforeBuild='static_files, css' />
var gulp = require("gulp"),
    sass = require("gulp-sass"),
    concat = require("gulp-concat"),
    rename = require("gulp-rename"),
    cleanCSS = require("gulp-clean-css"),
    merge2 = require("merge2");

var CSS_EXTENSION = ".css",
    SCSS_EXTENSION = ".scss";

var CSS_FOLDER_DEVELOPMENT = "Content/css/*" + SCSS_EXTENSION,
    STATICFILES_FOLDER_DEVELOPMENT = "Content/*",
    CSS_FOLDER_PRODUCTION = "wwwroot/css",
    STATICFILES_FOLDER_PRODUCTION = "wwwroot";

var CSS_FILE_DEVELOPMENT = "versus" + CSS_EXTENSION,
    CSS_FILE_PRODUCTION = "versus.min" + CSS_EXTENSION;

gulp.task("css", function () {
    return merge2(
        //gulp.src("node_modules/bootstrap/dist/css/bootstrap.css"),
        gulp.src(CSS_FOLDER_DEVELOPMENT)
    )
        .pipe(sass())
        .pipe(concat(CSS_FILE_DEVELOPMENT))
        .pipe(gulp.dest(CSS_FOLDER_PRODUCTION))
        .pipe(rename(CSS_FILE_PRODUCTION))
        .pipe(cleanCSS())
        .pipe(gulp.dest(CSS_FOLDER_PRODUCTION));
});

gulp.task("static_files", function () {
    // Move static files
    return gulp.src(STATICFILES_FOLDER_DEVELOPMENT, { nodir: true })
        .pipe(gulp.dest(STATICFILES_FOLDER_PRODUCTION));
});