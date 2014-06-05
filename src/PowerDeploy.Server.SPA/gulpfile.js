// Load plugins
var gulp = require('gulp'),
    sass = require('gulp-sass'),
    autoprefixer = require('gulp-autoprefixer'),
    minifycss = require('gulp-minify-css'),
    jshint = require('gulp-jshint'),
    uglify = require('gulp-uglify'),
    imagemin = require('gulp-imagemin'),
    rename = require('gulp-rename'),
    clean = require('gulp-clean'),
    concat = require('gulp-concat'),
    notify = require('gulp-notify'),
    cache = require('gulp-cache'),
    livereload = require('gulp-livereload'),
    inject = require('gulp-inject');

function startExpress() {
  var express = require('express');
  var app = express();
  app.use(express.static(__dirname));
  app.listen(1337);
}

gulp.task('styles', function() {
  return gulp.src('css/**/*.scss')
    .pipe(sass({ style: 'compact', sourcemap: true }))
    .pipe(autoprefixer('last 2 version', 'safari 5', 'ie 9', 'opera 12.1', 'ios 6', 'android 4'))
    .pipe(gulp.dest('css/'))
    //.pipe(gulp.dest('dist/css'))
    //.pipe(rename({ suffix: '.min' }))
    //.pipe(minifycss())
    //.pipe(gulp.dest('css/'))
    //.pipe(gulp.dest('dist/css'))
    //.pipe(notify({ message: 'Styles task complete' }));
});

gulp.task('scripts', function() {
  return gulp.src('js/**/*.js')
    .pipe(jshint('.jshintrc'))
    .pipe(jshint.reporter('jshint-stylish'))
    .pipe(concat('main.js'))
    .pipe(gulp.dest('dist/js'))
    .pipe(rename({ suffix: '.min' }))
    .pipe(uglify())
    .pipe(gulp.dest('dist/js'))
    //.pipe(notify({ message: 'Scripts task complete' }));
});

gulp.task('inject', function() {
   gulp.src('index.html')
  //.pipe(inject(gulp.src(['js/**/*.js'], {read: false})))
  .pipe(inject(gulp.src(['lib/**/*.js', '!lib/**/*.min.js'], {read: false}), { starttag: '<!-- inject:vendor:js -->' }))
  .pipe(inject(gulp.src(['js/**/*.js', '!js/**/*.min.js'], {read: false}), { starttag: '<!-- inject:angularapp:js -->' }))
  .pipe(inject(gulp.src(['css/**/*.css', '!css/**/*.min.css'], {read: false})))
  .pipe(gulp.dest(".")); 
});

//// Images
gulp.task('images', function() {
  return gulp.src('img/**/*')
    .pipe(cache(imagemin({ optimizationLevel: 3, progressive: true, interlaced: true })))
    .pipe(gulp.dest('dist/images'))
    //.pipe(notify({ message: 'Images task complete' }));
});

// Clean
gulp.task('clean', function() {
  return gulp.src(['dist/styles', 'dist/scripts', 'dist/images'], {read: false})
    .pipe(clean());
});

// Default task
gulp.task('default', ['clean'], function() {
    gulp.start('styles', 'images', 'inject', 'watch', 'server');
});

gulp.task('server', function () {
  startExpress();
});

// Watch
gulp.task('watch', function() {

  // Watch .scss files
  gulp.watch('css/**/*.scss', ['styles']);

  // Watch .js files
  //gulp.watch('js/**/*.js', ['scripts']);
    
  gulp.watch('lib/**/*.*', ['inject']);

  // Watch image files
  gulp.watch('img/**/*', ['images']);

  // Create LiveReload server
  var server = livereload();

  // Watch any files in dist/, reload on change
  gulp.watch(['js/**/*.*', 'index.html', 'css/**/*.*']).on('change', function(file) {
    console.log(file.path + ' changed');
    server.changed(file.path);
  });
});