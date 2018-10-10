/// <binding BeforeBuild="css, js" />
var gulp = require("gulp"),
    sass = require("gulp-sass"),
    concat = require("gulp-concat"),
    rename = require("gulp-rename"),
    terser = require("gulp-terser"),
    cleanCSS = require("gulp-clean-css"),
    babel = require("gulp-babel"),
    merge2 = require("merge2"),
    deletelines = require("gulp-delete-lines");

var JS_FOLDER = "wwwroot/js",
    CSS_FOLDER = "wwwroot/css";

var JSX_EXTENSION = ".jsx",
    JS_EXTENSION = ".js",
    CSS_EXTENSION = ".css";

var CSS_DEVELOPMENT = "versus" + CSS_EXTENSION,
    CSS_PRODUCTION = "versus.min" + CSS_EXTENSION,
    JS_LIBRARIES = "versus.libs" + JS_EXTENSION,
    JS_CUSTOMCODE = "versus" + JS_EXTENSION,
    JSX_SERVERSIDE = "versus" + JSX_EXTENSION,
    JS_PRODUCTION = "versus.min" + JS_EXTENSION;

gulp.task("css", function () {

    return merge2(
                gulp.src("node_modules/bootstrap/dist/css/bootstrap.css"),
                gulp.src("wwwroot/dev/css/*.scss")
            )
            .pipe(sass())
            .pipe(concat(CSS_DEVELOPMENT))
            .pipe(gulp.dest(CSS_FOLDER))
            .pipe(rename(CSS_PRODUCTION))
            .pipe(cleanCSS())
            .pipe(gulp.dest(CSS_FOLDER));
});

gulp.task("js", function () {

    // Bundle server-side JSX files for development
    var react = gulp.src("wwwroot/dev/js/*.jsx")
                    .pipe(deletelines({
                        filters: [
                            "import {.*;"
                        ]
                    }))
                    .pipe(concat(JSX_SERVERSIDE))
                    .pipe(gulp.dest(JS_FOLDER));

    // Bundle JS libraries for development
    var libs = merge2(
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
            .pipe(gulp.dest(JS_FOLDER)); 

    // Bundle custom JS with transformed JSX
    var js = merge2(
                gulp.src(["wwwroot/dev/js/*.js"
                ]),
                react
                    .pipe(babel({
                        presets: [
                            "@babel/preset-react"
                        ]
                    }))
            )
            .pipe(concat(JS_CUSTOMCODE))
            .pipe(gulp.dest(JS_FOLDER)); 

    // Bundle all files for production
    return merge2(libs, js)
            .pipe(concat(JS_PRODUCTION))
            .pipe(terser())
            .pipe(gulp.dest(JS_FOLDER));
});

