/// <binding BeforeBuild='css, static files, javascript' />
var gulp = require("gulp"),
    sass = require("gulp-sass"),
    concat = require("gulp-concat"),
    rename = require("gulp-rename"),
    terser = require("gulp-terser"),
    cleanCSS = require("gulp-clean-css"),
    babel = require("gulp-babel"),
    merge2 = require("merge2"),
    deletelines = require("gulp-delete-lines");

var JSX_EXTENSION = ".jsx",
    JS_EXTENSION = ".js",
    CSS_EXTENSION = ".css",
    SCSS_EXTENSION = ".scss";

var JS_FOLDER_DEVELOPMENT = "Client/js/*" + JS_EXTENSION,
    JSX_FOLDER_DEVELOPMENT = "Client/js/*" + JSX_EXTENSION,
    CSS_FOLDER_DEVELOPMENT = "Client/css/*" + SCSS_EXTENSION,
    STATICFILES_FOLDER_DEVELOPMENT = "Client/**",
    JS_FOLDER_PRODUCTION = "wwwroot/js",
    CSS_FOLDER_PRODUCTION = "wwwroot/css",
    STATICFILES_FOLDER_PRODUCTION = "wwwroot";


var CSS_FILE_DEVELOPMENT = "versus" + CSS_EXTENSION,
    CSS_FILE_PRODUCTION = "versus.min" + CSS_EXTENSION,
    JS_LIBRARIES = "versus.libs" + JS_EXTENSION,
    JS_FILE_CUSTOM = "versus" + JS_EXTENSION,
    JSX_FILE_SERVERSIDE = "versus" + JSX_EXTENSION,
    JS_FILE_PRODUCTION = "versus.min" + JS_EXTENSION;

gulp.task("css", function () {

    return merge2(
        gulp.src("node_modules/bootstrap/dist/css/bootstrap.css"),
        gulp.src(CSS_FOLDER_DEVELOPMENT)
    )
        .pipe(sass())
        .pipe(concat(CSS_FILE_DEVELOPMENT))
        .pipe(gulp.dest(CSS_FOLDER_PRODUCTION))
        .pipe(rename(CSS_FILE_PRODUCTION))
        .pipe(cleanCSS())
        .pipe(gulp.dest(CSS_FOLDER_PRODUCTION));
});

gulp.task("javascript", function () {

    // Bundle server-side JSX files for development
    var js_serverside = gulp.src(JSX_FOLDER_DEVELOPMENT)
        .pipe(deletelines({
            filters: [
                "import {.*;"
            ]
        }))
        .pipe(concat(JSX_FILE_SERVERSIDE))
        .pipe(gulp.dest(JS_FOLDER_PRODUCTION));

    // Bundle JS libraries for development
    var js_libraries = merge2(
        // Ensure core JS files are used first
        gulp.src(["node_modules/jquery/dist/jquery.slim.js",
            "node_modules/popper.js/dist/umd/popper.js",
            "node_modules/rxjs/bundles/rxjs.umd.js"
        ]),
        // Add in dependent JS files
        gulp.src([
            "node_modules/bootstrap/dist/js/bootstrap.bundle.js"
        ]))
        .pipe(concat(JS_LIBRARIES))
        .pipe(gulp.dest(JS_FOLDER_PRODUCTION));

    // Bundle custom JS with transformed JSX
    var js_development = merge2(
        gulp.src(JS_FOLDER_DEVELOPMENT),
        js_serverside
            .pipe(babel({
                presets: [
                    "@babel/preset-react"
                ]
            }))
        )
        .pipe(concat(JS_FILE_CUSTOM))
        .pipe(gulp.dest(JS_FOLDER_PRODUCTION));

    // Bundle all files for production
    return merge2(js_libraries, js_development)
        .pipe(concat(JS_FILE_PRODUCTION))
        .pipe(terser())
        .pipe(gulp.dest(JS_FOLDER_PRODUCTION));
});

gulp.task("static_files", function () {

    // Move static files
    return gulp.src(STATICFILES_FOLDER_DEVELOPMENT, { nodir: true })
        .pipe(gulp.dest(STATICFILES_FOLDER_PRODUCTION));
});

