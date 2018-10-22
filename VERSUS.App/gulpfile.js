///// <binding BeforeBuild='css, javascript, static_files' />
//var gulp = require("gulp"),
//    sass = require("gulp-sass"),
//    concat = require("gulp-concat"),
//    rename = require("gulp-rename"),
//    minify = require("gulp-terser"),
//    cleanCSS = require("gulp-clean-css"),
//    babel = require("gulp-babel"),
//    mergeStreams = require("merge2"),
//    removeLines = require("gulp-rm-lines"),
//    replace = require("gulp-replace");

//const JSX_EXTENSION = ".jsx",
//    JS_EXTENSION = ".js",
//    CSS_EXTENSION = ".css",
//    SCSS_EXTENSION = ".scss";

//const JS_FOLDER_DEVELOPMENT = "Client/js/*" + JS_EXTENSION,
//    JSX_FOLDER_DEVELOPMENT = "Client/js/*" + JSX_EXTENSION,
//    CSS_FOLDER_DEVELOPMENT = "Client/css/*" + SCSS_EXTENSION,
//    STATICFILES_FOLDER_DEVELOPMENT = "Client/*",
//    JS_FOLDER_PRODUCTION = "wwwroot/js",
//    CSS_FOLDER_PRODUCTION = "wwwroot/css",
//    STATICFILES_FOLDER_PRODUCTION = "wwwroot";


//const CSS_FILE_DEVELOPMENT = "versus" + CSS_EXTENSION,
//    CSS_FILE_PRODUCTION = "versus.min" + CSS_EXTENSION,
//    JS_LIBRARIES = "versus.libs" + JS_EXTENSION,
//    JS_FILE_CUSTOM = "versus" + JS_EXTENSION,
//    JSX_FILE_SERVERSIDE = "versus" + JSX_EXTENSION,
//    JS_FILE_PRODUCTION = "versus.min" + JS_EXTENSION;

//gulp.task("css", function () {

//    return mergeStreams(
//        gulp.src("node_modules/bootstrap/dist/css/bootstrap.css"),
//        gulp.src(CSS_FOLDER_DEVELOPMENT)
//    )
//        .pipe(sass())
//        .pipe(concat(CSS_FILE_DEVELOPMENT))
//        .pipe(gulp.dest(CSS_FOLDER_PRODUCTION))
//        .pipe(rename(CSS_FILE_PRODUCTION))
//        .pipe(cleanCSS())
//        .pipe(gulp.dest(CSS_FOLDER_PRODUCTION));
//});

//gulp.task("javascript", function () {

//    // Bundle server-side JSX files for development
//    var js_serverside =
//        //mergeStreams(


//        gulp.src(JSX_FOLDER_DEVELOPMENT)
//        //,
//        //gulp.src("node_modules/react-spring/dist/web.js")



//       // )
//        //.pipe(replace('process.env.NODE_ENV', '"development"'))
//        //.pipe(replace('global', 'window'))
//        //.pipe(removeLines({
//        //    filters: [
//        //        "import .*;",
//        //        "export .*;"
//        //    ]
//        //}))
//        //.pipe(concat(JSX_FILE_SERVERSIDE))
//        .pipe(gulp.dest("wwwroot/serverside"));

//    // Bundle JS libraries for development
//    var js_libraries = mergeStreams(
//        // Ensure core JS files are used first
//        gulp.src(["node_modules/jquery/dist/jquery.slim.js",
//            "node_modules/popper.js/dist/umd/popper.js",
//            "node_modules/rxjs/bundles/rxjs.umd.js"
//        ]),
//        // Add in dependent JS files
//        gulp.src([
//            "node_modules/bootstrap/dist/js/bootstrap.bundle.js"
//        ]))
//        .pipe(concat(JS_LIBRARIES))
//        .pipe(gulp.dest(JS_FOLDER_PRODUCTION));

//    // Bundle custom JS with transformed JSX
//    var js_development = mergeStreams(
//        gulp.src(JS_FOLDER_DEVELOPMENT),
//        js_serverside
//            .pipe(babel({
//                presets: [
//                    "@babel/preset-react"
//                ]
//            }))
//    );

//    // Separate piping because the merge below does not receive content of this stream
//    js_development.pipe(concat(JS_FILE_CUSTOM))
//        .pipe(gulp.dest(JS_FOLDER_PRODUCTION));

//    // Bundle all files for production
//    return mergeStreams(js_libraries, js_development)
//        .pipe(concat(JS_FILE_PRODUCTION))
//        .pipe(minify())
//        .pipe(gulp.dest(JS_FOLDER_PRODUCTION));
//});

//gulp.task("static_files", function () {

//    // Move static files
//    return gulp.src(STATICFILES_FOLDER_DEVELOPMENT, { nodir: true })
//        .pipe(gulp.dest(STATICFILES_FOLDER_PRODUCTION));
//});

