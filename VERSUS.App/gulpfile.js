/// <binding BeforeBuild="css, js" />
var gulp = require("gulp"),
    sass = require("gulp-sass"),
    concat = require("gulp-concat"),
    rename = require("gulp-rename"),
    terser = require("gulp-terser"),
    cleanCSS = require("gulp-clean-css"),
    babel = require("gulp-babel"),
    merge2 = require('merge2');

var JS_FOLDER = "wwwroot/js",
    CSS_FOLDER = "wwwroot/css";

gulp.task("css", function () {
    return gulp.src(["wwwroot/dev/css/*.scss", "node_modules/bootstrap/dist/css/bootstrap.css"])
        .pipe(sass())
        .pipe(concat("versus.css"))
        .pipe(gulp.dest(CSS_FOLDER))
        .pipe(rename("versus.min.css"))
        .pipe(cleanCSS())
        .pipe(gulp.dest(CSS_FOLDER));
});

gulp.task("js", function () {

    // Bundle JSX files for development
    var jsx = gulp.src("wwwroot/dev/js/*.jsx")
        .pipe(babel({
            presets: [
                ["@babel/preset-env",
                    {
                        targets:
                        {
                            chrome: "58",
                            ie: "11"
                        }
                    }
                ],
                "@babel/preset-react"
            ]
        }))
        .pipe(concat("versus-jsx.js"))
        .pipe(gulp.dest(JS_FOLDER));

    // Ensure core JS files are used first
    var jsDependencies = gulp.src(["node_modules/jquery/dist/jquery.slim.js",
                                    "node_modules/popper.js/dist/umd/popper.js",
                                    "node_modules/rxjs/bundles/rxjs.umd.js"
                                ]);

    // Add in dependent JS files
    var jsCustom = gulp.src(["wwwroot/dev/js/*.js",
                             "node_modules/bootstrap/dist/js/bootstrap.bundle.js"
                            ]);

    // Bundle JS files for development
    var js = merge2(jsDependencies, jsCustom)
        .pipe(concat("versus.js"))
        .pipe(gulp.dest(JS_FOLDER)); 

    // Bundle JS and JSX files for production
    return merge2(jsx, js)
        .pipe(concat("versus.min.js"))
        .pipe(terser())
        .pipe(gulp.dest(JS_FOLDER));
});

