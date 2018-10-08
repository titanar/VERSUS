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

gulp.task("css", function () {


    return merge2(
                gulp.src("node_modules/bootstrap/dist/css/bootstrap.css"),
                gulp.src("wwwroot/dev/css/*.scss")
            )
            .pipe(sass())
            .pipe(concat("versus.css"))
            .pipe(gulp.dest(CSS_FOLDER))
            .pipe(rename("versus.min.css"))
            .pipe(cleanCSS())
            .pipe(gulp.dest(CSS_FOLDER));
});

gulp.task("js", function () {

    // Rework bundling so that dependency libraries are first and only minified, then custom JS, then transpiled JSX


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
            .pipe(concat("versus-libs.js"))
        .pipe(gulp.dest(JS_FOLDER)); 


    // Bundle JSX files for development
    var react = gulp.src("wwwroot/dev/js/*.jsx")
        .pipe(deletelines({
            filters: [
                "import {.*;"
            ]
        }))
        .pipe(concat("versus.jsx"))
        .pipe(gulp.dest(JS_FOLDER));
;

    var js = merge2(
                gulp.src(["wwwroot/dev/js/*.js"
                ]),
        react
            .pipe(babel({
                presets: [
                    //"@babel/preset-env",
                    "@babel/preset-react"
                ]
            }))
                    
                    
                    
             )
            .pipe(concat("versus.js"))
            .pipe(gulp.dest(JS_FOLDER)); 

    // Bundle JS and JSX files for production
    return merge2(libs, js)
        .pipe(concat("versus.min.js"))
        .pipe(terser())
        .pipe(gulp.dest(JS_FOLDER));
});

